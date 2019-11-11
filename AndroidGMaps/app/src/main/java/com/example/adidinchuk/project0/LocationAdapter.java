package com.example.adidinchuk.project0;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.fragment.app.FragmentActivity;
import androidx.recyclerview.widget.RecyclerView;

import org.w3c.dom.Text;

import java.util.List;

public class LocationAdapter extends RecyclerView.Adapter<LocationAdapter.MyViewHolder> implements View.OnClickListener {
    private List<StoredLocation> mDataset;
    private MapsActivity context;
    private ItemClickListener mClickListener;
    // Provide a reference to the views for each data item
    // Complex data items may need more than one view per item, and
    // you provide access to all the views for a data item in a view holder
    public static class MyViewHolder extends RecyclerView.ViewHolder {
        // each data item is just a string in this case
        public View textView;
        public MyViewHolder(View v) {
            super(v);
            textView = v;
        }
    }

    @Override
    public void onClick(View view) {
        if (mClickListener != null) mClickListener.onItemClick(view, 0);
    }

        // Provide a suitable constructor (depends on the kind of dataset)
    public LocationAdapter(List<StoredLocation> myDataset, MapsActivity context) {
        mDataset = myDataset;
        this.context = context;
    }

    // Create new views (invoked by the layout manager)
    @Override
    public LocationAdapter.MyViewHolder onCreateViewHolder(ViewGroup parent,
                                                           int viewType) {
        // create a new view
        View v = (View) LayoutInflater.from(parent.getContext())
                .inflate(R.layout.recycle_view, parent, false);

        MyViewHolder vh = new MyViewHolder(v);
        return vh;
    }

    // Replace the contents of a view (invoked by the layout manager)
    @Override
    public void onBindViewHolder(MyViewHolder holder, final int position) {
        // - get element from your dataset at this position
        // - replace the contents of the view with that element
        ((TextView)holder.textView.findViewById(R.id.locationLabel)).setText(mDataset.get(position).label);
        ((TextView)holder.textView.findViewById(R.id.locationSnippet)).setText(mDataset.get(position).snippet);
        ((TextView)holder.textView.findViewById(R.id.locationLabel)).setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                context.zoomOnMarker(mDataset.get(position).index);
            }
        });

        ((Button)holder.textView.findViewById(R.id.deleteLocation)).setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                removeItem(position);
            }
        });
    }

    // Return the size of your dataset (invoked by the layout manager)
    @Override
    public int getItemCount() {
        return mDataset.size();
    }

    public void removeItem(int index){
        context.removeMarker(mDataset.get(index).index);
        mDataset.remove(index);
        notifyItemRemoved(index);
        notifyItemRangeChanged(index, getItemCount());
        Data.getInstance(context).saveData(context);
    }



    public interface ItemClickListener {
        void onItemClick(View view, int position);
    }
}