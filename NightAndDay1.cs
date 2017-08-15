using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controles the sky and sound in order to mimic the setting and rising of the sun, 
/// rotation of the heavenly spheres, and day and night ambient sounds.
///     SCENE REQUIREMENTS: This script requires that the game has 4 DIRECTIONAL LIGHTS, 1 PARTICLE SYSTEM,
/// 1 SKYBOX, and 3 AUDIO SOURCES. 
///     The directional light for the sun should be far away from the play area (e.g., position [0,500,0]). 
/// For best effect, the sky should be default, or a skybox with an equivalently dynamic sun. This sun directional light
/// should be set as the sun for the skybox (this can be done in the Lighting settings for the scene). The directional light
/// for the moon should be placed at a downward angle towards the scene, with a blue tent. The two other directional lights
/// ('front light' and 'back light') should be place at downward angles towards the scene on opposite corners of the scene. 
/// These lights will be extra sunlight and thus should be the same color as the sun.
///     For best effect, the stars particle system should be a sphere shape with a radius of 800 with "Emit From 
/// Shell" checked. Its "Rate Over Time" should be set to 5000 and its "Max Particles" should be 0, or at least a 
/// small number. For variation, I recommend setting the "Start Color' to a random color between white and a light blue,
/// setting the "Start Size" to between 5 and 14, and setting the "Start Lifetime" to between 360 and 1000 seconds. If you 
/// desire smaller and/or more numerous stars, increase the radius of the particle system and number of particles generated
/// by the modifyStarNum function. However, if the particle system radius is increased, the camera's clipping plane Far variable
/// will also need to be increased so it can see the stars.
///     There are no requirements for the audio sources, other than they should all be set to "Loop." For best effect, set 
/// birdChirp1 and birdChirp2 to two different unobtrusive bird sounds. Also, set nightAmbience to quiet night sounds such
/// as wind or crickets.
///     
/// TO MAKE THIS SCRIPT FUNCTION PROPERLY, PLACE IT ON THE SUN DIRECTIONAL LIGHT IN YOUR SCENE. However, this can be 
/// easily modified in the code, if desired. Also, set the Sky Speed variable (in the script's component) to 5 and then 
/// adjust it until you have the desired speed.
/// </summary>
public class NightAndDay1 : MonoBehaviour
{

    public GameObject moon;             //The moon light
    public GameObject stars;            //The Stars particle system
    public Light frontLight, backLight; //The ambient lights for the day
    public Material sky;                //The sky in th world
    public AudioSource birdChirp1;      //The first looping sound clip of birds
    public AudioSource birdChirp2;      //The second looping sound clip of birds
    public AudioSource nightAmbience;   //The looping sound clip of night animals
    public float skySpeed;              //The speed at which the sun and moon and stars rotate
    private float atmosphere;           //The atmosphere thickness
    private float daySoundFade;         //The rate at which the day sound fades out and in
    private float nightSoundFade;       //The rate at which the night sound fades out and in
    private int angleDif;               //The difference between the past and current angles
    private int currentAngleY;          //The current angle of the Sun object along the y axis
    private int pastAngleY;             //The last angle before most recent modification

    /// <summary>
    /// Adds or subtracts the number of stars (Max Particles) based
    /// on the position of the sun near the horizon, while also modifying
    /// the intensity of the sun, moon, and other directional lights.
    /// </summary>
    private void modifyStarNum()
    {
        pastAngleY = currentAngleY;                                 //Sets the past angle to what the current angle used to be and
        currentAngleY = (int)transform.position.y;                 //updates the current angle.


        if (transform.position.y < 200 && transform.position.y >= -50 && transform.position.z > 0)  //if the sun is setting.
        {
            if (!stars.activeSelf)                                                  //turns on the stars at sunrise.
            {
                stars.SetActive(true);
            }
            if (stars.GetComponent<ParticleSystem>().main.maxParticles >= 4500)     //breaks out if there are 1000 stars.
            {
                stars.GetComponent<ParticleSystem>().maxParticles = 5000;           //Sets the number of stars to 1000.
                return;
            }
            if (currentAngleY != pastAngleY)                                        //if there was a change in the sun angle since last call.
            {
                angleDif = (int)Mathf.Abs(pastAngleY - currentAngleY);             //The difference between the angles.
                stars.GetComponent<ParticleSystem>().maxParticles += 25 * angleDif; //Adds 5 stars for every degree the angle has changed since last check.

                GetComponent<Light>().intensity -= 0.008f;
                moon.GetComponent<Light>().intensity += 0.005f;
                frontLight.intensity -= 0.004f;
                backLight.intensity -= 0.004f;
            }
        }
        else if (transform.position.y < 200 && transform.position.y >= -50 && transform.position.z < 0)   //if the sun is rising.
        {
            if (stars.GetComponent<ParticleSystem>().main.maxParticles <= 500)       //breaks out if there are no more stars to subtract
            {
                stars.GetComponent<ParticleSystem>().maxParticles = 1;              //Sets the number of stars to 1.
                stars.SetActive(false);                                             //Turns off the stars completely until sunrise.
                return;
            }
            if (currentAngleY != pastAngleY)                                        //if there was a change in the sun angle since last call.
            {
                angleDif = (int)Mathf.Abs(pastAngleY - currentAngleY);              //The difference between the angles.
                stars.GetComponent<ParticleSystem>().maxParticles -= 25 * angleDif;  //Subtracts 5 stars for every degree the angle has changed since last check.

                GetComponent<Light>().intensity += 0.008f;
                moon.GetComponent<Light>().intensity -= 0.005f;
                frontLight.intensity += 0.004f;
                backLight.intensity += 0.004f;
            }
        }
    }

    /// <summary>
    /// modifies atmosphere, sky tent, and exposure as the sun sets.
    /// </summary>
    private void modifySky()
    {
        if (transform.position.y < 200 && transform.position.y >= -30 && transform.position.z > 0)  //if the sun is setting.
        {
            if (currentAngleY != pastAngleY)                                            //if there was a change in the sun angle since last call.
            {
                angleDif = (int)Mathf.Abs(pastAngleY - currentAngleY);                  //The difference between the angles.
                atmosphere += .005f;                                                    //Increases the variable by a set amount.
                RenderSettings.skybox.SetFloat("_AtmosphereThickness", atmosphere);     //Increases the scene's atmosphere thickness by the set amount.
            }
        }
        else if (transform.position.y < 200 && transform.position.y >= -30 && transform.position.z < 0)   //if the sun is rising.
        {
            if (currentAngleY != pastAngleY)                                            //if there was a change in the sun angle since last call.
            {
                angleDif = (int)Mathf.Abs(pastAngleY - currentAngleY);                  //The difference between the angles.
                atmosphere -= .005f;                                                    //Decreases the variable by a set amount.
                RenderSettings.skybox.SetFloat("_AtmosphereThickness", atmosphere);     //Decreases the scene's atmosphere thickness by the set amount.
            }
        }
    }

    /// <summary>
    /// modifies ambient sound by slowly tranforming from day nature sounds
    /// to night nature sounds and then back again, based on time of day
    /// </summary>
    private void modifySound()
    {
        if (transform.position.y < 200 && transform.position.y >= -50 && transform.position.z > 0)  //if the sun is setting.
        {
            if (daySoundFade <= 0 || nightSoundFade >= .5)           //when either sound has reached its proper volume for the night,
            {                                                       //all sounds go to there proper volume.
                birdChirp1.volume = 0;
                birdChirp2.volume = 0;
                nightAmbience.volume = .5f;
                return;
            }

            if (currentAngleY != pastAngleY)                        //if there was a change in the sun angle since last call.
            {
                angleDif = (int)Mathf.Abs(pastAngleY - currentAngleY);  //The difference between the angles.

                daySoundFade -= .002f;                              //decreases the variable by a set amount.
                nightSoundFade += .002f;                            //increases the variable by a set amount.

                birdChirp1.volume = daySoundFade;                   //decreases the day ambience by the set amount.
                birdChirp2.volume = daySoundFade;
                nightAmbience.volume = nightSoundFade;              //increases the night ambience by the set amount.
            }
        }
        else if (transform.position.y < 100 && transform.position.y >= -150 && transform.position.z < 0)   //if the sun is rising.
        {
            if (daySoundFade >= .5 || nightSoundFade <= 0)          //when either sound has reached its proper volume for the night,
            {                                                       //all sounds go to there proper volume.
                birdChirp1.volume = .5f;
                birdChirp2.volume = .5f;
                nightAmbience.volume = 0;
                return;
            }

            if (currentAngleY != pastAngleY)                            //if there was a change in the sun angle since last call.
            {
                angleDif = (int)Mathf.Abs(pastAngleY - currentAngleY);  //The difference between the angles.

                daySoundFade += .002f;                                  //increases the variable by a set amount.
                nightSoundFade -= .002f;                                //decreases the variable by a set amount.

                birdChirp1.volume = angleDif * daySoundFade;            //increases the day ambience by the set amount.
                birdChirp2.volume = angleDif * daySoundFade;
                nightAmbience.volume = angleDif * nightSoundFade;       //decreases the night ambience by the set amount.
            }
        }
    }
    /// <summary>
    /// Rotates the sun, moon directional lights and the stars 
    /// particle system around the scene and a specified, set speed.
    /// </summary>
    private void SkyRotation()
    {
        transform.LookAt(Vector3.zero);
        transform.RotateAround(Vector3.zero, Vector3.right, skySpeed * Time.deltaTime);
        stars.transform.RotateAround(Vector3.zero, Vector3.right, skySpeed * Time.deltaTime);
    }

    // Use this for initialization
    void Start()
    {
        currentAngleY = (int)transform.position.y;     //Sets the variable to the sun's current angle.
        atmosphere = 1;                                 //Initializes the variable to the actual game amosphere thickness.
        birdChirp1.volume = .5f;                        //Initializes the volume to its starting amount.
        birdChirp2.volume = .5f;                        //Initializes the volume to its starting amount.
        nightAmbience.volume = .0f;                     //Initializes the volume to its starting amount.
        daySoundFade = birdChirp1.volume;               //Sets the variable to the current day sound volume.
        nightSoundFade = nightAmbience.volume;          //Sets the variable to the current night sound volume.
        RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1);  //makes sure the amosphere thickness begins at 1.
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SkyRotation();
        modifyStarNum();
        modifySky();
        modifySound();
    }
}
