//Attach this script to the Game Object you're using as your First Person Controller. Only include one instance of this sciprt as a component in each scene of the game.
//Mod by Ricardo A Moreno for Sealth Game
using UnityEngine;

public class FMODStudioThirdPersonFootsteps : MonoBehaviour {
    //These variables will all be set in the Inspector tab of Unity's Editor by either us, or the 'FMODStudioFootstepsEditor' script.
    [Header ("FMOD Settings")]
    [SerializeField][FMODUnity.EventRef] private string FootstepsEventPath; // Use this in the Editor to select our Footsteps Event.
    [SerializeField][FMODUnity.EventRef] private string CrouchEventPath;
    [SerializeField] private string MaterialParameterName; // Use this in the Editor to write the name of the parameter that contorls which material the player is currently walking on.
    [SerializeField] private string SpeedParameterName; // Use this in the Editor to write the name of the parameter that contorls which footstep speed needs to be heard.
    [Header ("Playback Settings")]
    [SerializeField] private string CrouchInputName;
    [SerializeField] private string SpeedInputName;

    public string[] MaterialTypes; // This is an array of strings. In the inspector we can decide how many Material types we have in FMOD by setting the size of this array. Depending on the size, the array will then create a certain amount of strings for us to fill in with the name of each of our footstep materials for our scripts to use. This will then remain a constant and will not change.
    [HideInInspector] public int DefulatMaterialValue; // This will be told by the 'FMODStudioFootstepsEditor' script which Material has been set as the defualt. It will then store the value of that Material for outhis script to use. This cannot be changed in the Editor, but a drop down menu created by the 'FMODStudioFootstepsEditor' script can.
    private int F_MaterialValue; // We'll use this to set the value of our FMOD Material Parameter.
    private int F_PlayerRunning; // We'll use to set the value of our FMOD Switch Speed Parameter.

    void Update () // This method, and everyting inside of it, is perfomred on the very first frame that is run when we start the game.
    {
        if (Input.GetButtonDown (CrouchInputName)) // If the player is touching the ground AND the player presses the button to jump at the same time, we knoe that the player character is about to jump, therefore we perform our method to play a sound.
        {
            PlayCrouchEvent (); // The 'PlayJumpOrLand' method is perfomred, triggering our 'Jump & Land' event in FMOD to play. We also pass through it's parameter brackets the 'true' boolean value for our method to store inside a vaiable and use to play a jump sound with.
        }
        if (Input.GetButton (SpeedInputName)) // If the player is touching the ground AND the player presses the button to jump at the same time, we knoe that the player character is about to jump, therefore we perform our method to play a sound.
        {
            F_PlayerRunning = 1; // The 'PlayJumpOrLand' method is perfomred, triggering our 'Jump & Land' event in FMOD to play. We also pass through it's parameter brackets the 'true' boolean value for our method to store inside a vaiable and use to play a jump sound with.
        } else {
            F_PlayerRunning = 0;
        }
    }

    void Step () // This method, and everyting insid of it, is perfomred once every frame. If the game is running at an average of 60 frames per second, this method will be perfomred 60 times a second.
    {
        PlayFootstep (); // The PlayFootstep method is performed and a footstep audio file from FMOD is played!
    }

    private void OnTriggerEnter (Collider hit) {
        if (hit.gameObject.GetComponent<FMODStudioMaterialSetter> ()) // Using the 'hit' varibale, we check to see if the raycast has hit a collider attached to a gameobject, that also has the 'FMODStudioMaterialSetter' script attached to it as a component...
        {
            F_MaterialValue = hit.gameObject.GetComponent<FMODStudioMaterialSetter> ().MaterialValue; // ...and if it did, we then set our 'F_MaterialValue' varibale to match whatever value the 'MaterialValue' variable (which is inside the 'F_MaterialValue' varibale) is currently set to.
        } else // Else if however, the player is standing on an object that doesn't have a 'FMODStudioMaterialSetter' script component for our raycast to find...
            F_MaterialValue = DefulatMaterialValue; // ...we then set 'F_MaterialValue' to match the value of 'DefulatMaterialValue'. 'DefulatMaterialValue' is given a value by the 'FMODStudioFootstepsEditor' script. This value represents whatever material we have selected as our 'DefulatMaterial' in the Unity Inspector tab.
    }

    private void OnTriggerExit (Collider hit) {
        F_MaterialValue = DefulatMaterialValue;
        //Debug.Log (floorTag);
    }

    void PlayFootstep () // When this method is performed, our footsteps event in FMOD will be told to play.
    {
        FMOD.Studio.EventInstance Footstep = FMODUnity.RuntimeManager.CreateInstance (FootstepsEventPath); // If they are, we create an FMOD event instance. We use the event path inside the 'FootstepsEventPath' variable to find the event we want to play.
        FMODUnity.RuntimeManager.AttachInstanceToGameObject (Footstep, transform, GetComponent<Rigidbody> ()); // Next that event instance is told to play at the location that our player is currently at.
        Footstep.setParameterByName (MaterialParameterName, F_MaterialValue); // Before the event is played, we set the Material Parameter to match the value of the 'F_MaterialValue' variable.
        Footstep.setParameterByName (SpeedParameterName, F_PlayerRunning); // We also set the Speed Parameter to match the value of the 'F_PlayerRunning' variable.
        Footstep.start (); // We then play a footstep!.
        Footstep.release (); // We also set our event instance to release straight after we tell it to play, so that the instance is released once the event had finished playing.
    }

    void PlayCrouchEvent () // When this method is performed our Jumping And Landing event in FMOD will be told to play. A boolean variable is also created inside it's parameter brackets to be used inside this method. This variable will hold whichever value we gave this method when we called it in the Update function.
    {
        FMOD.Studio.EventInstance Crouch = FMODUnity.RuntimeManager.CreateInstance (CrouchEventPath); // First we create an FMOD event instance. We use the event path inside the 'JumpingEventPath' variable to find the event we want to play.
        FMODUnity.RuntimeManager.AttachInstanceToGameObject (Crouch, transform, GetComponent<Rigidbody> ()); // Next that event instance is told to play at the location that our player is currently at.
        Crouch.start (); // We then play our event, and the player either hear's a jumping or a landing sound!
        Crouch.release (); // We also set our event instance to release straight after we tell it to play, so that the instance is released once the event had finished playing.
    }
}