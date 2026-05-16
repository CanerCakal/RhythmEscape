using UnityEngine;

public static class SelectedMusicManager
{
    public static MusicTrackData SelectedTrack { get; private set; }

    public static void SelectTrack(MusicTrackData trackData)
    {
        SelectedTrack = trackData;

        if (trackData != null)
        {
            Debug.Log("Seçilen müzik: " + trackData.trackName + " | BPM: " + trackData.bpm);
        }
    }

    public static bool HasSelectedTrack()
    {
        return SelectedTrack != null;
    }
}