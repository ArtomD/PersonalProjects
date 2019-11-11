package com.example.adidinchuk.project0;


import java.io.Serializable;

public class StoredLocation implements Serializable {
    public transient static int currentIndex = -1;
    private static final long serialVersionUID = 1L;
    public double lat;
    public double lng;
    public String label;
    public String snippet;
    public int index;

    public StoredLocation(double lat, double lng, String label, String snippet){
        this.label = label;
        this.lat = lat;
        this.lng = lng;
        this.snippet = snippet;
        currentIndex++;
        this.index = currentIndex;
    }

}
