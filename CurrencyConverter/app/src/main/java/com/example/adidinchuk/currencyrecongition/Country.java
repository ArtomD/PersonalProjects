package com.example.adidinchuk.currencyrecongition;

public class Country implements Comparable<Country> {
    String CountryName;
    String CountryCode;
    String CurrencyName;
    String CurrencyCode;
    double ratioToUSD;

    //all currencies are compared to a base value(USD) and stored as a ratio to it
    //this function returns the relative ratios of two different currencies
    public double convert(Country o){
       return o.ratioToUSD/this.ratioToUSD;
    }

    //objects are sorted by CurrencyName as that is what they are displayed as in the spinner
    @Override
    public int compareTo(Country o) {
        return this.CurrencyName.compareToIgnoreCase(o.CurrencyName);
    }
    //spinner will display the currency name
    @Override
    public String toString() {
        return CurrencyName;
    }
}
