package com.example.adidinchuk.currencyrecongition;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentTransaction;

import android.content.pm.PackageManager;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.ColorFilter;
import android.graphics.PorterDuff;
import android.graphics.PorterDuffColorFilter;
import android.os.Bundle;
import android.util.JsonReader;
import android.view.Gravity;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.FirebaseApp;
import com.google.firebase.ml.common.FirebaseMLException;
import com.google.firebase.ml.common.modeldownload.FirebaseModelDownloadConditions;
import com.google.firebase.ml.common.modeldownload.FirebaseModelManager;
import com.google.firebase.ml.custom.FirebaseModelInterpreter;
import com.google.firebase.ml.vision.FirebaseVision;
import com.google.firebase.ml.vision.automl.FirebaseAutoMLLocalModel;
import com.google.firebase.ml.vision.automl.FirebaseAutoMLRemoteModel;
import com.google.firebase.ml.vision.common.FirebaseVisionImage;
import com.google.firebase.ml.vision.label.FirebaseVisionImageLabel;
import com.google.firebase.ml.vision.label.FirebaseVisionImageLabeler;
import com.google.firebase.ml.vision.label.FirebaseVisionOnDeviceAutoMLImageLabelerOptions;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class MainActivity extends AppCompatActivity implements CameraFragment.OnCameraFragmentGetResult, AdapterView.OnItemSelectedListener {

    //camera fragment managment
    FragmentManager fragmentManager;
    CameraFragment cameraFragment;
    private boolean cameraActive;

    //views accessed by activity
    Spinner currentCountryDropdown;
    Spinner targetCountryDropdown;

    TextView currentCountryName;
    TextView currentCountryCurrencyCode;
    TextView targetCountryName;
    TextView targetCountryCurrencyCode;
    TextView currencyRatio;
    Button cameraButton;

    //list to populate spinners
    List<Country> currencies;
    //firebase stuff
    private FirebaseModelInterpreter mInterpreter;
    private FirebaseVisionOnDeviceAutoMLImageLabelerOptions options;
    private FirebaseAutoMLLocalModel localModel;
    private FirebaseAutoMLRemoteModel remoteModel;
    private FirebaseVisionImageLabeler labeler;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //view objects
        setContentView(R.layout.activity_main);
        currentCountryDropdown = findViewById(R.id.CurrentCountry);
        targetCountryDropdown = findViewById(R.id.TargetCountry);
        currentCountryName = findViewById(R.id.currentCountryName);
        currentCountryCurrencyCode = findViewById(R.id.currentCountryCurrency);
        targetCountryName = findViewById(R.id.targetCountryName);
        targetCountryCurrencyCode = findViewById(R.id.targetCountryCurrency);
        currencyRatio = findViewById(R.id.currencyRatio);
        cameraButton = findViewById(R.id.showCameraButton);
        cameraActive = false;

        setupFirebaseModel();
        populateCurrencySpinners();
        //fragment setup
        fragmentManager = getSupportFragmentManager();
        cameraFragment = new CameraFragment();
    }
    //populats spinners from asset JSON file
    private void populateCurrencySpinners(){
        currencies = new ArrayList<Country>();
        InputStream currenicies = null;
        try {
            currenicies = getAssets().open("currencies.json");
            JsonReader reader = new JsonReader(new InputStreamReader(currenicies));
            reader.beginObject();
            reader.nextName();
            reader.beginArray();
            while (reader.hasNext()) {
                Country country = new Country();
                reader.beginObject();
                reader.nextName();
                country.CountryName = reader.nextString();
                reader.nextName();
                country.CountryCode = reader.nextString();
                reader.nextName();
                country.CurrencyName = reader.nextString();
                reader.nextName();
                country.CurrencyCode = reader.nextString();
                reader.endObject();
                currencies.add(country);
            }
            reader.endArray();
            reader.endObject();
        } catch (IOException e) {
            e.printStackTrace();
        }

        Collections.sort(currencies);
        ArrayAdapter<Country> adapter =
                new ArrayAdapter<Country>(getApplicationContext(),  android.R.layout.simple_spinner_dropdown_item, currencies);
        adapter.setDropDownViewResource( android.R.layout.simple_spinner_dropdown_item);


        currentCountryDropdown.setAdapter(adapter);
        targetCountryDropdown.setAdapter(adapter);

        currentCountryDropdown.setOnItemSelectedListener(this);
        targetCountryDropdown.setOnItemSelectedListener(this);

        currentCountryDropdown.setSelection(currencies.size()-1);

        currencyRatio.setText("Loading");
        getCurrencyRatio();
    }

    //init firebase model
    private void setupFirebaseModel(){
        FirebaseApp.initializeApp(this);

        localModel = new FirebaseAutoMLLocalModel.Builder()
                .setAssetFilePath("currencyModel/manifest.json")
                .build();

        remoteModel = new FirebaseAutoMLRemoteModel.Builder("CurrencyAll_20191116115644").build();

        FirebaseModelDownloadConditions conditions = new FirebaseModelDownloadConditions.Builder()
                .requireWifi()
                .build();
        FirebaseModelManager.getInstance().download(remoteModel, conditions)
                .addOnCompleteListener(new OnCompleteListener<Void>() {
                    @Override
                    public void onComplete(@NonNull Task<Void> task) {
                        //model loaded remote
                    }
                });
        //downloading remote model
        FirebaseModelManager.getInstance().isModelDownloaded(remoteModel)
                .addOnSuccessListener(new OnSuccessListener<Boolean>() {
                    @Override
                    public void onSuccess(Boolean isDownloaded) {
                        FirebaseVisionOnDeviceAutoMLImageLabelerOptions.Builder optionsBuilder;
                        if (isDownloaded) {
                            optionsBuilder = new FirebaseVisionOnDeviceAutoMLImageLabelerOptions.Builder(remoteModel);
                        } else {
                            optionsBuilder = new FirebaseVisionOnDeviceAutoMLImageLabelerOptions.Builder(localModel);
                        }
                        options = optionsBuilder
                                .setConfidenceThreshold(0.45f)
                                .build();
                        try {
                            labeler = FirebaseVision.getInstance().getOnDeviceAutoMLImageLabeler(options);
                        } catch (FirebaseMLException e) {
                            // Error.
                        }
                    }
                });

        //using local model
        try {
            FirebaseVisionOnDeviceAutoMLImageLabelerOptions options =
                    new FirebaseVisionOnDeviceAutoMLImageLabelerOptions.Builder(localModel)
                            .setConfidenceThreshold(0.45f)
                            .build();
            labeler = FirebaseVision.getInstance().getOnDeviceAutoMLImageLabeler(options);
        } catch (FirebaseMLException e) {
            // ...
        }
    }

    //get current curency ratios from API with async thread
    private void getCurrencyRatio(){
        new Thread(new CountryRatioRunnable(this) ).start();
    }

    //thread to get currency ratios
    private class CountryRatioRunnable implements Runnable {

        MainActivity context;
        public CountryRatioRunnable(MainActivity context) {
            this.context = context;
        }
        public void run() {
            try {
                context.setupHTTPCall();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
    //does the HTTP call and sends result to processing function
    private void setupHTTPCall() throws IOException {

        URL url = new URL("http://www.apilayer.net/api/live?access_key="+getResources().getString(R.string.API_KEY));

        HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
        try {
            InputStream in = new BufferedInputStream(urlConnection.getInputStream());
            String output = readStream(in);
            onCurrencyRatioAquired(output);
        } finally {
            urlConnection.disconnect();
        }
    }
    //reads input stream and returns a string
    private String readStream(InputStream input) throws IOException {
        BufferedReader r = new BufferedReader(new InputStreamReader(input));
        StringBuilder result = new StringBuilder();
        for (String line; (line = r.readLine()) != null; ) {
            result.append(line);
        }
        return result.toString();
    }
    //parses received JSON from string
    //populates stored currency objects in list
    //refreshes view
    private void onCurrencyRatioAquired(String response) {
        try {
            JSONObject jObject = new JSONObject(response);
            for(Country country : currencies){
                country.ratioToUSD = jObject.getJSONObject("quotes").getDouble("USD"+country.CurrencyCode);
            }
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    displayCurrencyRatio();
                }
            });
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    //callback for spinners when a currency is selected
    @Override
    public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
        displayCurrencyRatio();
    }
    //updates the view based on spinner data selected
    private void displayCurrencyRatio(){
        Country currentCountry = (Country) currentCountryDropdown.getSelectedItem();
        Country targetCountry = (Country) targetCountryDropdown.getSelectedItem();
        currentCountryName.setText(currentCountry.CountryName + " (" + currentCountry.CountryCode +")");
        currentCountryCurrencyCode.setText(currentCountry.CurrencyCode);
        targetCountryName.setText(targetCountry.CountryName + " (" + targetCountry.CountryCode +")");
        targetCountryCurrencyCode.setText(targetCountry.CurrencyCode);
        String ratio = "1 : " + currentCountry.convert(targetCountry);
        if(ratio.length()>10){
            ratio = ratio.substring(0,10);
        }
        currencyRatio.setText(ratio);
    }
    //mandatory unused method
    @Override
    public void onNothingSelected(AdapterView<?> parent) {

    }
    //permission request callback, if permission is granted procedes with opening fragment
    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String[] permissions, int[] grantResults) {
        if(grantResults[0]==PackageManager.PERMISSION_GRANTED){
            launchCameraFragment();
        }
    }
    //function called from open/close camera button
    //opens and closes the camera fragment is permissions are met, asks for permissions if otherwise
    public void toggleCameraFragment(View view){
        if(cameraActive){
           removeCameraFramgment();
        }else{
            if (ContextCompat.checkSelfPermission(this,
                    android.Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this,
                        new String[]{android.Manifest.permission.CAMERA}, 1);
            }else{
                launchCameraFragment();
            }
        }
    }

    //opens the camerafragment and updates the open/close camera button
    private void launchCameraFragment(){
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
        fragmentTransaction.add(R.id.cameraFragmentView, cameraFragment);
        fragmentTransaction.commit();
        cameraButton.setText(getResources().getString(R.string.cameraButtonTurnOff));
        cameraActive = true;
    }

    //closes the camerafragment and updates the open/close camera button
    private void removeCameraFramgment(){
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
        fragmentTransaction.remove(cameraFragment);
        fragmentTransaction.commit();
        cameraButton.setText(getResources().getString(R.string.cameraButtonTurnOn));
        cameraActive=false;
    }

    //callback from camera fragment that recieves the image taken
    //runs firebase recognition on it.
    @Override
    public void sendBitmapImage(Bitmap image) {

        try {
            FirebaseVisionImage fimage = FirebaseVisionImage.fromBitmap(image);

            labeler.processImage(fimage)
                    .addOnSuccessListener(new OnSuccessListener<List<FirebaseVisionImageLabel>>() {
                        @Override
                        public void onSuccess(List<FirebaseVisionImageLabel> labels) {
                            if (labels.size() > 0) {
                                int index = -1;
                                for (int position=0; position<targetCountryDropdown.getAdapter().getCount(); position++){
                                    if (((Country)targetCountryDropdown.getAdapter().getItem(position)).CountryName.toLowerCase().equals(labels.get(0).getText())) {
                                        int color = android.graphics.Color.argb(255, 15, 15, 15);
                                        int background = android.graphics.Color.argb(255, 215, 215, 215);
                                        makeToast(((Country) targetCountryDropdown.getAdapter().getItem(position)).CurrencyName + " found.", Toast.LENGTH_SHORT, background,color);
                                        index = position;
                                        break;
                                    }
                                }
                                if(index!=-1) {
                                    targetCountryDropdown.setSelection(index);
                                    displayCurrencyRatio();
                                }else{
                                    int color = android.graphics.Color.argb(255, 15, 15, 15);
                                    int background = android.graphics.Color.argb(255, 225, 170, 170);
                                    makeToast("No currency found, please try again.", Toast.LENGTH_SHORT,background ,color);
                                }
                            } else {
                                int color = android.graphics.Color.argb(255, 15, 15, 15);
                                int background = android.graphics.Color.argb(255, 225, 170, 170);
                                makeToast("No currency found, please try again.", Toast.LENGTH_SHORT,background ,color);
                            }
                        }
                    })
                    .addOnFailureListener(new OnFailureListener() {
                        @Override
                        public void onFailure(@NonNull Exception e) {
                        }
                    });


        } catch (Exception ee) {
        }
    }

    //sends toast notification about firebase recognition results
    private void makeToast(String text, int duration, int backgroundColor, int textColor){
        Toast toast = Toast.makeText(getApplicationContext(), text, duration);
        View view = toast.getView();
        view.getBackground().setColorFilter(new PorterDuffColorFilter(backgroundColor, PorterDuff.Mode.SRC_IN));
        TextView textView = view.findViewById(android.R.id.message);
        textView.setTextColor(textColor);
        toast.setGravity(Gravity.CENTER,0,0);
        toast.show();
    }
}



