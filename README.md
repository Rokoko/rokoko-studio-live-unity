<h1 align="center">Rokoko Studio Live Plugin for Unity</h1>

[Rokoko Studio](https://www.rokoko.com/en/products/studio) is a powerful and intuitive software for recording, visualizing and exporting motion capture.

This plugin let's you stream animation data from Rokoko Studio into Unreal Engine to preview and work with all your motion capture data easily and intuitively.

---

## Requirements
The minimum officially supported version of Unity is 2018.4.x LTS. Although it hasn't been tested for earlier versions, we except that all core scripts should work as we use pure C#, but not the UI functionallity.

## Features
- Live stream data:
  * Up to five actors that can all include all body, face (52 blendshapes) and finger data at the same time.
  * Camera data
  * Props data
- Control Rokoko Studio from within Unity

---

# Getting Started

### Package includes 2 demo scenes

1. RokokoPluginExampleScene is a very basic setup that re-creates the Studio data with default actors and props. Ideal for testing out the plugin.
2. RokokoPluginExampleScene_CustomActors is an extended demo that demonstrates the usage of custom actors and props. Please expand *CustomInputs* game object in hierarchy to see various different examples.

### Top level scripts and settings

#### 1. StudioManager
StudioManager script is responsible for listening and populating the scene with actors and props from the incoming RokokoStudio data.

* __Receive Port__ - This is the listening port for RokokoStudio data. It should match the selected port of the Live Stream option inside RokokoStudio. 14043 is by default.
* __Actor and Prop Prefab__ - These two fields are __optional__ and used to auto-generate Actors and Props when no overrides found.
* __UI Manager__ - This is an __optional__ field for showing in UI all the available inputs coming from RokokStudio live data.
* __Actor and Prop Overrides__ - These fields are auto-generated based on the Actor and Prop overrides found in scene. This is a read only field to check the overrides during runtime
* __AutoGenerateInputsWhenNoOverridesFound__ - You can toggle on/off whether the StudioManager should auto-generate Actors and Props when no overrides founds.
* __ShowDefaultActorWhenNoData__ - This shows the default Actor Prefab in T Pose when no RokokoStudio data is availble.

#### 2. CommandAPI
CommandAPI script is responsible for communicating remote commands to RokokoStudio such as StartRecording, StopRecording, Calibrate Smartsuit etc.
* __API Key__ - This key must match the API Key in RokokoStudio *(Menu->Settings->CommandAPI->API Key)*.
* __Port__ - This is the send port for communicating commands and should match the port in RokokoStudio *(Menu->Settings->CommandAPI->Listen Port)*.
* __Response Text__ - This is an __optional__ field for showing the command status.
* __IP Address__ - This is the local IP of RokokoStudio. If it's on the same machine, leave it as 127.0.0.1. Alternatively you can assign it to a different local network address.

### Overriding Actors and Props with custom assets
#### 1. Actor
You can import any custom character in Unity and use it as an Actor. Simply add your character into your scene and add __*Actor*__ component on it.

__Important Notice: Any custom character/actor must be in a T Pose, so we can retarget it properly from Smartsuit.__
* __Profile Name__ - Update this field with the profile name of RokokStudio to override this actor with your custom one.
* __Bone Mapping__
* * __Animator__ - If your character is in the stardard Humanoid format you have to mark it as __*Humanoid*__ in the __*Animation Type*__ under the __*Rig*__ tab in Unity's inspector.
* * __Custom__ - By selecting this option a new script __*HumanBoneMapping*__ is automatically added and assigned in the __*GameObject*__. From there you may assign any custom hierarchy to be driven by the Actor's data.
* __Face__ - Assign any __*Face*__ for the given __*Actor*__

#### 2. Face
Face does not override a profile directly. Instead it's controlled by an __*Actor*__
* __Blendshape Mapping__
* * __ARKit__ - Use this type if your face model is rigged according to Apple's [ARKit Blendshapes](./Assets/Rokoko/Scripts/Core/ARKitBlendshapes.cs) 
* * __Custom__ - By selecting this option a new script __*BlendshapesMapping*__ is automatically added and assigned in the __*GameObject*__. From there you may assign any custom blendshape names to be driven by the Face's data.
* * > __Note:__ You can see your model's existing blendshapes by expanding the *Blendshapes* menu inside the __*SkinnedMeshRenderer*__ of your face model.
* __MeshRenderer__ - Here you assign your face model's __*SkinnedMeshRenderer*__

#### 3. Prop
You may add any kind of 3D object in the scene and add the __*Prop*__ script on it.
* __Prop Name__ - Update this field with the prop name of RokokStudio to override this prop with your custom one.
* __MeshRenderer__ - Here you assign your face model's __*SkinnedMeshRenderer*__

---

# FAQ

#####  1. Incoming data are in bad format. Please ensure you are using JSON v3 as forward data format
Make sure you are using RokokoStudio 1.8 and above and you have selected Unity as the live forward option. Alternatively you can use custom forward data, go to its settings, expand the details dropdown and select JSON v3 as the Data Format.

#####  2. Seem like port: xxxxx is already in use. Is plugin running already in other application?
Make sure that the plugin is not already running in another instance, or the selected binding port is not used by another application.

#####  3. My character has wrong joint rotations
Make sure that your character is in T Pose. Please refer to __Actor__ section.

---

# Licence
[GNU Lesser General Public License v3.0](./LICENSE.md)
