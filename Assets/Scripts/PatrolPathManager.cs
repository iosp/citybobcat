using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Display Mode that the Custom Inspector of an PatrolPathManager
// component can be in
public enum PathDisplayMode { None, Connections, Paths }

// -------------------------------------------------------------------
// CLASS	:	PatrolPathManager
// DESC		:	Contains a list of waypoints. Each waypoint is a 
//				reference to a transform. Also contains settings
//				for the Custom Inspector
// ------------------------------------------------------------------
public class PatrolPathManager : MonoBehaviour
{
   
    public PathDisplayMode DisplayMode = PathDisplayMode.Connections;   // Current Display Mode
   
    public int UIStart = 0;                                         // Start wayopoint index for Paths mode

    public int UIEnd = 0;                                           // End waypoint index for Paths mode

    // List of Transform references
    public List<Transform> Waypoints = new List<Transform>();

}
