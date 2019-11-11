package com.example.adidinchuk.project0;

import androidx.annotation.ColorInt;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.FragmentActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.location.Location;
import android.os.Build;
import android.os.Bundle;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.PopupWindow;


import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.BitmapDescriptorFactory;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.location.LocationServices;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

public class MapsActivity extends FragmentActivity implements OnMapReadyCallback, GoogleApiClient.ConnectionCallbacks,
        GoogleApiClient.OnConnectionFailedListener {

    private RecyclerView actualRecyclerView;
    private RecyclerView.Adapter actualMAdapter;
    private RecyclerView.LayoutManager actualLayoutManager;

    private RecyclerView wantedRecyclerView;
    private RecyclerView.Adapter wantedMAdapter;
    private RecyclerView.LayoutManager wantedLayoutManager;

    private GoogleApiClient googleApiClient;
    private GoogleMap mMap;
    private Data data;
    private Marker currentMarker;
    private HashMap<Integer,Marker> savedMarkers;
    private FusedLocationProviderClient fusedLocationClient;

    ReadWriteLock lock = new ReentrantReadWriteLock();
    private LatLng currentLocation;
    private boolean cameraMoved = false;

    private PopupWindow mPopupWindow;
    private Context mContext;
    private LinearLayout mainLayout;

    private Button newCurrentLocation;
    private Button newCustomLocation;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        //setup
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_maps);
        mContext = getApplicationContext();
        //popup setup
        mainLayout = (LinearLayout) findViewById(R.id.MainMapLayout);
        newCurrentLocation = (Button) findViewById(R.id.new_current_button);
        newCustomLocation = (Button) findViewById(R.id.new_custom_button);

        newCurrentLocation.setOnClickListener(new NewMarkerListener(this,true));
        newCustomLocation.setOnClickListener(new NewMarkerListener(this,false));


        //data setup
        data = Data.getInstance(this);

        //recycleview setup
        actualRecyclerView = findViewById(R.id.actualLocationList);
        actualRecyclerView.setHasFixedSize(true);

        actualLayoutManager = new LinearLayoutManager(this);
        actualRecyclerView.setLayoutManager(actualLayoutManager);

        actualMAdapter = new LocationAdapter(data.lists.beenLocations,this);
        actualRecyclerView.setAdapter(actualMAdapter);


        wantedRecyclerView = findViewById(R.id.wantedLocationList);
        wantedRecyclerView.setHasFixedSize(true);

        wantedLayoutManager = new LinearLayoutManager(this);
        wantedRecyclerView.setLayoutManager(wantedLayoutManager);

        wantedMAdapter = new LocationAdapter(data.lists.wantedLocations,this);
        wantedRecyclerView.setAdapter(wantedMAdapter);

        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);
        // Checks if ACCESS_FINE_LOCATION permission is granted
        if (ContextCompat.checkSelfPermission(this,
                android.Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this,
                    new String[]{android.Manifest.permission.ACCESS_FINE_LOCATION}, 1);
        }
        // Initialize the Google API Client
        googleApiClient = new GoogleApiClient.Builder
                (this, this, this)
                .addApi(LocationServices.API).build();
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(this);
    }


    public void goToAboutPage(View view){
        Intent intent = new Intent(this,
                AboutPage.class);
        startActivity(intent);
    }



    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
    @Override
    public void onMapReady(GoogleMap googleMap) {

        mMap = googleMap;
        //make sure data is loaded in case this runs before onCreate
        data = Data.getInstance(this);
        //init markers
        savedMarkers = new HashMap<Integer,Marker>();
        //load markers
        for(StoredLocation loc : data.lists.wantedLocations){
            addMarkerToMap(loc,BitmapDescriptorFactory.HUE_AZURE);
        }
        for(StoredLocation loc : data.lists.beenLocations){
            addMarkerToMap(loc,BitmapDescriptorFactory.HUE_GREEN);
        }
    }

    @Override
    public void onConnected(@Nullable Bundle bundle) {
        //set current position and launch thread updating it
        lock.writeLock().lock();
        try {
            currentLocation = new LatLng(0,0);
            currentMarker = mMap.addMarker(new MarkerOptions().position(currentLocation).icon(BitmapDescriptorFactory.defaultMarker(BitmapDescriptorFactory.HUE_RED))
                    .title("Current Location").snippet("Lat:"+currentLocation.latitude+",Lng:"+currentLocation.longitude).alpha(0.4f));
        } finally {
            lock.writeLock().unlock();
        }

        launchTrackingThread();
    }

    public void trackCurrentLocation(){
        fusedLocationClient.getLastLocation().addOnSuccessListener(this, new OnSuccessListener<Location>() {
            @Override
            public void onSuccess(Location lastLocation) {
                if (lastLocation != null) {
                    double lat = lastLocation.getLatitude(), lon = lastLocation.getLongitude();
                    lock.writeLock().lock();
                    try {
                        currentLocation = new LatLng(lat, lon);
                    } finally {
                        lock.writeLock().unlock();
                    }
                    lock.readLock().lock();
                    try {
                        currentMarker.setPosition(currentLocation);
                        currentMarker.setSnippet("Lat:"+currentLocation.latitude+",Lng:"+currentLocation.longitude);
                    } finally {
                        lock.readLock().unlock();
                    }

                    //move camera once at start
                    if(!cameraMoved){
                        cameraMoved = true;
                        lock.readLock().lock();
                        try {
                            mMap.moveCamera(CameraUpdateFactory.newLatLng(currentLocation));
                            mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(currentLocation, 14));
                        } finally {
                            lock.readLock().unlock();
                        }
                    }
                }
            }

        }).addOnFailureListener(this, new OnFailureListener() {
            @Override
            public void onFailure(@NonNull Exception e) {
                System.out.println(e);
            }
        });
    }

    private void launchTrackingThread(){
        Thread thread = new Thread() {
            @Override
            public void run() {
                try {
                    while(true) {
                        trackCurrentLocation();
                        sleep(3500);

                    }
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        };

        thread.start();
    }



    //methods to interact with map and markers
    private void addMarkerToMap(StoredLocation loc, float color){
        savedMarkers.put(loc.index,mMap.addMarker(new MarkerOptions().position(new LatLng(loc.lat,loc.lng)).icon(BitmapDescriptorFactory.defaultMarker(color))
                .title(loc.label).snippet(loc.snippet).alpha(0.8f)));
    }

    public void removeMarker(int index){
        savedMarkers.get(index).remove();
        savedMarkers.remove(index);
    }

    public void zoomOnMarker(int index){
        mMap.moveCamera(CameraUpdateFactory.newLatLng(savedMarkers.get(index).getPosition()));
        mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(savedMarkers.get(index).getPosition(), 14));
    }

    //listener for add marker buttons
    public class NewMarkerListener implements View.OnClickListener {
        private MapsActivity context;
        private boolean isCurrentLocation;


        public NewMarkerListener(MapsActivity context, boolean isCurrentLocation){
            this.context = context;
            this.isCurrentLocation = isCurrentLocation;
        }

        @Override
        public void onClick(View view) {
            // Initialize a new instance of LayoutInflater service
            LayoutInflater inflater = (LayoutInflater) mContext.getSystemService(LAYOUT_INFLATER_SERVICE);
            View customView = inflater.inflate(R.layout.create_marker,null);
            mPopupWindow = new PopupWindow(
                    customView,
                    LinearLayout.LayoutParams.WRAP_CONTENT,
                    LinearLayout.LayoutParams.WRAP_CONTENT,true
            );
            // Set an elevation value for popup window
            // Call requires API level 21
            if(Build.VERSION.SDK_INT>=21){
                mPopupWindow.setElevation(5.0f);
            }

            Button saveButton = (Button) customView.findViewById(R.id.new_marker_save);
            Button closeButton = (Button) customView.findViewById(R.id.new_marker_cancel);

            final EditText newMarkerLabel = (EditText) customView.findViewById(R.id.new_marker_label);
            final EditText newMarkerSnippet = (EditText) customView.findViewById(R.id.new_marker_snippet);
            final EditText newMarkerLat = (EditText) customView.findViewById(R.id.new_marker_lat);
            final EditText newMarkerLng = (EditText) customView.findViewById(R.id.new_marker_lng);
            final LinearLayout layout = (LinearLayout) customView.findViewById(R.id.popupLayout);
            if(isCurrentLocation){
                lock.readLock().lock();
                try {
                    newMarkerLat.setText(Double.toString(currentLocation.latitude));
                    newMarkerLng.setText(Double.toString(currentLocation.longitude));
                } finally {
                    lock.readLock().unlock();
                }


                newMarkerLat.setEnabled(false);
                newMarkerLng.setEnabled(false);

                layout.setBackgroundColor(Color.parseColor("#FFE3FCBE"));
            }else{
                newMarkerLat.setText(Double.toString(mMap.getCameraPosition().target.latitude));
                newMarkerLng.setText(Double.toString(mMap.getCameraPosition().target.longitude));
                newMarkerLat.setEnabled(true);
                newMarkerLng.setEnabled(true);
                layout.setBackgroundColor(Color.parseColor("#FFCFECFA"));
            }

            // Set a click listener for the popup window close button
            saveButton.setOnClickListener(new SaveMarkerListener(context,isCurrentLocation,newMarkerLat,newMarkerLng,newMarkerLabel,newMarkerSnippet));



            closeButton.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {

                    mPopupWindow.dismiss();
                }
            });

            // Finally, show the popup window at the center location of root relative layout
            mPopupWindow.showAtLocation(mainLayout, Gravity.CENTER,0,0);
        }
    }
//listener used inside popup window launched in previous listener
    public class SaveMarkerListener implements View.OnClickListener {

        MapsActivity context;
        boolean isCurrentLocation;
        EditText newMarkerLat;
        EditText newMarkerLng;
        EditText newMarkerLabel;
        EditText newMarkerSnippet;

        public SaveMarkerListener(MapsActivity context, boolean isCurrentLocation, EditText newMarkerLat,EditText newMarkerLng,EditText newMarkerLabel,EditText newMarkerSnippet){
            this.context = context;
            this.isCurrentLocation = isCurrentLocation;
            this.newMarkerLabel = newMarkerLabel;
            this.newMarkerLat = newMarkerLat;
            this.newMarkerLng = newMarkerLng;
            this.newMarkerSnippet = newMarkerSnippet;
        }

        @Override
        public void onClick(View view) {
            Data data = Data.getInstance(context);
            StoredLocation newLocation = new StoredLocation(Double.parseDouble(newMarkerLat.getText().toString()),Double.parseDouble(newMarkerLng.getText().toString()),newMarkerLabel.getText().toString(),newMarkerSnippet.getText().toString());
            if(isCurrentLocation){
                data.lists.beenLocations.add(newLocation);
                actualMAdapter.notifyDataSetChanged();
            }else{
                data.lists.wantedLocations.add(newLocation);
                wantedMAdapter.notifyDataSetChanged();
            }
            data.saveData(context);
            context.addMarkerToMap(newLocation,isCurrentLocation?BitmapDescriptorFactory.HUE_GREEN:BitmapDescriptorFactory.HUE_AZURE);
            mPopupWindow.dismiss();
        }
    }


   //lifecycle methods

    @Override
    protected void onStart(){
        super.onStart();
        if(googleApiClient  != null){
            googleApiClient.connect();
        }
    }

    @Override
    protected void onStop(){
        googleApiClient.disconnect();
        super.onStop();
    }

    //unused methods
    @Override
    public void onConnectionSuspended(int i) {

    }

    @Override
    public void onConnectionFailed(@NonNull ConnectionResult connectionResult) {

    }

    @Override
    public void onPointerCaptureChanged(boolean hasCapture) {

    }
}
