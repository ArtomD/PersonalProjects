package com.example.adidinchuk.project0;

import android.content.Context;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;

public class FileUtils {

    public static LocationLists loadLocations(int filename, Context context) throws IOException, ClassNotFoundException {

        File file = context.getFileStreamPath(context.getResources().getString(filename));
        if(file == null || !file.exists()) {
            return new LocationLists();
        }

        FileInputStream fis = context.openFileInput(context.getResources().getString(filename));
        ObjectInputStream is = new ObjectInputStream(fis);
        LocationLists data = (LocationLists) is.readObject();
        is.close();
        fis.close();

        return data;
    }

    public static void saveLocation(LocationLists data, int filename,Context context) throws IOException {
        FileOutputStream fos = context.openFileOutput(context.getResources().getString(filename), Context.MODE_PRIVATE);
        ObjectOutputStream os = new ObjectOutputStream(fos);
        os.writeObject(data);
        os.close();
        fos.close();
    }
}
