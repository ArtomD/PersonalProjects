package com.example.adidinchuk.project0;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class LocationLists implements Serializable {

    public List<StoredLocation> beenLocations;
    public List<StoredLocation> wantedLocations;
    private static final long serialVersionUID = 1L;

    public LocationLists(){
        this.beenLocations = new ArrayList<StoredLocation>();
        this.wantedLocations = new ArrayList<StoredLocation>();
    }
}
