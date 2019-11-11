package com.example.adidinchuk.project0;

import android.content.Context;

import com.google.android.gms.maps.model.Marker;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.util.ArrayList;
import java.util.List;

public class Data {
    private static Data instance;
    public final static int filename = R.string.location_filename;
    public LocationLists lists;
    private Data(){}
    private void populateInstance(Context context){
        try {
            instance.lists = FileUtils.loadLocations(filename,context);
            int lastIndex = -1;
            for(StoredLocation loc : lists.beenLocations){
                if(loc.index>lastIndex)
                    lastIndex = loc.index;
            }
            for(StoredLocation loc : lists.wantedLocations){
                if(loc.index>lastIndex)
                    lastIndex = loc.index;
            }
        StoredLocation.currentIndex = lastIndex;
        } catch (IOException e) {

            e.printStackTrace();
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        }
    }

    public static Data getInstance(Context context){
        if(instance == null){
            instance = new Data();
            instance.populateInstance(context);
        }
        return instance;
    }


    public void saveData(Context context){
        try {
            FileUtils.saveLocation(instance.lists,filename,context);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}
