using Gamekit3D;
using UnityEngine;

public class SoundFader : MonoBehaviour
{
    public SoundTrack soundtrack;

    public void FadeIn ()
    {
        soundtrack.soundTrackVolume = 1;
    }

    public void FadeOut ()
    {
        soundtrack.soundTrackVolume = 0;
    }
}
