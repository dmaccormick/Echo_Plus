using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using System.Text;
using System.Collections.Generic;
using Thesis.Interface;
using Thesis.Recording;

namespace Thesis.RecTrack
{
    // Currently only fully works with a single animation layer
    // TODO: Add full for multiple animation layers!
    [RequireComponent(typeof(Recording_Object))]
    public class RecTrack_Skeletal : MonoBehaviour, IRecordable
    {
        //--- Data Structs ---//
        [System.Serializable]
        public struct Data_Skeletal_Setup
        {
            public Data_Skeletal_Setup(bool _useDefaultJointTransforms, bool _applyRootMotion, RuntimeAnimatorController _animatorController, Avatar _avatar, Transform _targetRigRoot, SkinnedMeshRenderer[] _targetSkins)
            {
                this.m_useDefaultJointTransforms = _useDefaultJointTransforms;
                this.m_applyRootMotion = _applyRootMotion;
                this.m_animatorController = _animatorController;
                this.m_animationAvatar = _avatar;
                this.m_targetRigRoot = _targetRigRoot;
                this.m_targetSkins = _targetSkins;

#if !UNITY_EDITOR
                this.m_studyObj = FindObjectOfType<Study_AssetPaths>();
                Debug.Log(this.m_studyObj);
#endif
            }

            public string GetString()
            {
                return m_useDefaultJointTransforms.ToString() + "~"
                    + m_applyRootMotion.ToString() + "~"
                    + GetFullRigString() + "~"
                    + GetFullSkinString() + "~"
#if UNITY_EDITOR
                    + AssetDatabase.GetAssetPath(this.m_animatorController) + "~"
                    + AssetDatabase.GetAssetPath(this.m_animationAvatar);
#else
                    + m_studyObj.GetPathForObject(this.m_animatorController) + "~"
                    + m_studyObj.GetPathForObject(this.m_animationAvatar);
#endif
            }

            public string GetFullRigString()
            {
                // Get all of the bones in the rig and store them in a list
                List<Transform> rigBones = new List<Transform>(m_targetRigRoot.GetComponentsInChildren<Transform>());

                // Extract the names for all of the bones
                List<string> rigBoneNames = new List<string>();
                foreach (var bone in rigBones)
                    rigBoneNames.Add(bone.name);

                // Extract the indices for the parents of all of the bones. This can be used later to reconstruct the hierarchy
                List<int> rigBoneParentIndices = new List<int>();
                foreach (var bone in rigBones)
                {
                    // If the bone's parent is not in the list (ie: it is the root), just use an index of -1
                    if (rigBones.Contains(bone.parent))
                        rigBoneParentIndices.Add(rigBones.IndexOf(bone.parent));
                    else
                        rigBoneParentIndices.Add(-1);
                }

                // Ensure there is the correct number of names and indicies
                Assert.IsTrue(rigBoneNames.Count == rigBoneParentIndices.Count, "The number of rig bones and rig parent indices needs to be equal in RecTrack_Skeletal");

                // Combine the list of bone names and parent indices into a single string
                // Also add the local positions, rotations, and scales to help setup the initial skeleton (use locals since the skeleton is entirely built around parenting and lossy values can get messed up during linking)
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < rigBoneNames.Count && i < rigBoneParentIndices.Count; i++)
                {
                    //stringBuilder.Append(rigBoneNames[i] + "`" + rigBoneParentIndices[i].ToString() + ",");
                    stringBuilder.Append(rigBoneNames[i] + "`");
                    stringBuilder.Append(rigBoneParentIndices[i].ToString() + "`");
                    stringBuilder.Append(rigBones[i].position.ToString("F10") + "`");
                    stringBuilder.Append(rigBones[i].rotation.ToString("F10") + "`");
                    stringBuilder.Append(rigBones[i].lossyScale.ToString("F10"));
                    stringBuilder.Append("$");
                }

                // Return the final string value
                return stringBuilder.ToString();
            }

            public string GetFullSkinString()
            {
                // Create a string builder to hold the list of skins
                StringBuilder stringBuilder = new StringBuilder();

                // Combine the mesh and material information for each of the renderers
                foreach (var skinnedMesh in m_targetSkins)
                {
                    // Add the mesh information first
                    int meshSubIndex = GetMeshSubAssetIndex(skinnedMesh.sharedMesh);

#if UNITY_EDITOR
                    stringBuilder.Append(AssetDatabase.GetAssetPath(skinnedMesh.sharedMesh) + "`" + meshSubIndex.ToString() + ";");
#else
                    stringBuilder.Append(m_studyObj.GetPathForObject(skinnedMesh.sharedMesh) + "`" + meshSubIndex.ToString() + ";");
#endif
                    // Add the various materials afterwards
                    foreach (var mat in skinnedMesh.sharedMaterials)
                    {
#if !UNITY_EDITOR
                        string matPath = m_studyObj.GetPathForObject(mat);
#else
                        string matPath = AssetDatabase.GetAssetPath(mat);

                        if (matPath == "" || matPath == null || matPath == " ")
                        {
                            // Output a warning message to indicate that we couldn't find a matching material
                            string matName = mat.name;

                            // If the material is an instance, we can search for the original
                            if (matName.Contains("(Instance)"))
                            {
                                // Grab all the GUIDs for every material in the database
                                string[] allMatGUIDs = AssetDatabase.FindAssets("t:Material");

                                // Look for the start of the (Instance) indicator from the material name
                                int instanceStartIdx = matName.IndexOf('(');
                                matName = matName.Substring(0, instanceStartIdx - 1);

                                // Search all of the materials in the database to determine if one of them has the same name. If it is, it should be the base version of the material we found
                                foreach (var matGUID in allMatGUIDs)
                                {
                                    string matObjPath = AssetDatabase.GUIDToAssetPath(matGUID);

                                    if (matObjPath.Contains(matName))
                                    {
                                        matPath = matObjPath;
                                        break;
                                    }
                                }
                            }
                        }
#endif
                        // If we couldn't find a matching material even while searching the database, output a message
                        if (matPath == "" || matPath == null || matPath == " ")
                            Debug.LogWarning("Warning in RecTrack_Skeletal: the mat path could not be found for mat object: " + mat.name);

                        stringBuilder.Append(matPath + "`");
                    }

                    // Add a separator before listing the joints
                    stringBuilder.Append(';');

                    // Add the indices for the joints as well
                    var fullRig = new List<Transform>(m_targetRigRoot.GetComponentsInChildren<Transform>());
                    var thisRendererJoints = skinnedMesh.bones;
                    foreach (var joint in thisRendererJoints)
                    {
                        // Determine the index of the joint in the main rig list
                        int jointIndex = fullRig.IndexOf(joint);

                        // Add the index to the string
                        stringBuilder.Append(jointIndex.ToString() + "`");
                    }

                    // Add a final separator to split between the skinned meshes
                    stringBuilder.Append(',');
                }

                // Return the final string
                return stringBuilder.ToString();
            }

            private int GetMeshSubAssetIndex(Mesh _mesh)
            {
#if UNITY_EDITOR
                string meshPath = AssetDatabase.GetAssetPath(_mesh);
                var listOfObjects = AssetDatabase.LoadAllAssetsAtPath(meshPath);
#else
                string meshPath = m_studyObj.GetPathForObject(_mesh);
                var listOfObjects = Resources.LoadAll(meshPath);
#endif

                int meshCount = 0;
                int thisMeshIndex = -1;

                // Find how many meshes there are in the list and grab the index of this mesh specificially as well
                for (int i = 0; i < listOfObjects.Length; i++)
                {
                    var meshConversionAttempt = listOfObjects[i] as Mesh;

                    if (meshConversionAttempt != null)
                    {
                        meshCount++;

                        if (meshConversionAttempt == _mesh)
                            thisMeshIndex = i;
                    }
                }

                // If this is the only mesh, just return -1
                // Otherwise, return this mesh's index
                return (meshCount > 1) ? thisMeshIndex : -1;
            }

            public bool m_useDefaultJointTransforms;
            public bool m_applyRootMotion;
            public RuntimeAnimatorController m_animatorController;
            public Avatar m_animationAvatar;
            public Transform m_targetRigRoot;
            public SkinnedMeshRenderer[] m_targetSkins;

#if !UNITY_EDITOR
            private Study_AssetPaths m_studyObj;
#endif
        }


        [System.Serializable]
        public struct Data_Skeletal_Anim
        {
            public Data_Skeletal_Anim(float _timestamp, int _animHash, float _animTime)
            {
                this.m_timestamp = _timestamp;
                this.m_animHash = _animHash;
                this.m_animTime = _animTime;
            }

            public string GetString(string _format)
            {
                return this.m_timestamp.ToString(_format) + "~" + this.m_animHash.ToString() + "~" + this.m_animTime.ToString(_format);
            }

            public float m_timestamp;
            public int m_animHash;
            public float m_animTime;
        }



        //--- Public Variables ---//
        [Header("Recording Settings")]
        public Recording_Settings m_recordingSettings;

        [Header("Skeletal Rig")]
        public Transform m_targetRigRoot;
        public SkinnedMeshRenderer[] m_targetSkins;
        public bool m_useDefaultJointTransforms;

        [Header("Animation")]
        public Animator m_targetAnimator;
        public bool m_applyRootMotionInVis;



        //--- Private Variables ---//
        private Data_Skeletal_Setup m_skeletalSetupData;
        private List<Data_Skeletal_Anim> m_animDataPoints;



        //--- IRecordable Interfaces ---//
        public void StartRecording(float _startTime)
        {
            // Ensure the targets are set
            Assert.IsNotNull(m_targetAnimator, "Track Assert Failed [" + GetTrackName() + "] - " + "m_targetAnimator needs to be set for the track on object [" + this.gameObject.name + "]");
            Assert.IsNotNull(m_targetRigRoot, "Track Assert Failed [" + GetTrackName() + "] - " + "m_targetRigRoot needs to be set for the track on object [" + this.gameObject.name + "]");
            Assert.IsNotNull(m_targetSkins, "Track Assert Failed [" + GetTrackName() + "] - " + "m_targetSkins needs to be set for the track on object [" + this.gameObject.name + "]");

            // Init the private variables
            m_animDataPoints = new List<Data_Skeletal_Anim>();

            // Record the information about the skeleton
            m_skeletalSetupData = new Data_Skeletal_Setup(m_useDefaultJointTransforms, m_applyRootMotionInVis, m_targetAnimator.runtimeAnimatorController, m_targetAnimator.avatar, m_targetRigRoot, m_targetSkins);

            // Record the first animation data point
            RecordData(_startTime);
        }

        public void EndRecording(float _endTime)
        {
            // Record the final data point
            RecordData(_endTime);
        }

        public void UpdateRecording(float _currentTime)
        {
            // Handle the different styles of recording
            if (m_recordingSettings.m_recordingMethod == Recording_Method.On_Change)
            {
                // Get the previously recorded datapoint and the current data point
                Data_Skeletal_Anim lastDataPoint = m_animDataPoints[m_animDataPoints.Count - 1];
                AnimatorStateInfo currentStateInfo = m_targetAnimator.GetCurrentAnimatorStateInfo(0);

                // If in a completely new animation, we should definitely record
                if (currentStateInfo.shortNameHash != lastDataPoint.m_animHash)
                    RecordData(_currentTime);

                // Otherwise, we should compare the time difference
                float timeDifference = currentStateInfo.normalizedTime - lastDataPoint.m_animTime;

                // If the difference is significant enough, we should record the data
                if (timeDifference >= m_recordingSettings.m_changeMinThreshold)
                {
                    // Record the current data point
                    RecordData(_currentTime);
                }
            }
            else if (m_recordingSettings.m_recordingMethod == Recording_Method.Every_X_Seconds)
            {
                // If enough time has passed, update the recording
                if (_currentTime >= m_recordingSettings.m_nextSampleTime)
                    RecordData(_currentTime);
            }
            else
            {
                // Always record data when doing the every frame recording
                RecordData(_currentTime);
            }
        }

        public void RecordData(float _currentTime)
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_animDataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints must be init before calling RecordData() on object [" + this.gameObject.name + "]");

            // Get the data from the animator
            AnimatorStateInfo stateInfo = m_targetAnimator.GetCurrentAnimatorStateInfo(0);

            // Add the animation datapoint to the list
            m_animDataPoints.Add(new Data_Skeletal_Anim(_currentTime, stateInfo.shortNameHash, stateInfo.normalizedTime));

            // Recalculate the next sample time
            m_recordingSettings.m_nextSampleTime = _currentTime + m_recordingSettings.m_sampleTime;
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_animDataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints must be init before calling GetData() on object [" + this.gameObject.name + "]");

            // Use a string builder to compile the data string efficiently
            StringBuilder stringBuilder = new StringBuilder();

            // Always get the string data from the skeletal information first
            stringBuilder.AppendLine(m_skeletalSetupData.GetString());

            // Add all of the animation datapoints to the string with the requested format
            foreach (Data_Skeletal_Anim data in m_animDataPoints)
                stringBuilder.AppendLine("\t\t" + data.GetString(m_recordingSettings.m_dataFormat));

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }

        public string GetTrackName()
        {
            return "Skeletal";
        }

        public void SetupDefault()
        {
            // Setup this recording track by grabbing default values from this object
            m_targetAnimator = GetComponent<Animator>();
            m_targetSkins = GetComponents<SkinnedMeshRenderer>();

            // If either one failed, try to grab from the children or the parent instead
            m_targetAnimator = (m_targetAnimator == null) ? GetComponentInChildren<Animator>() : m_targetAnimator;
            m_targetAnimator = (m_targetAnimator == null) ? GetComponentInParent<Animator>() : m_targetAnimator;
            m_targetSkins = (m_targetSkins == null) ? GetComponentsInChildren<SkinnedMeshRenderer>() : m_targetSkins;
            m_targetSkins = (m_targetSkins == null) ? GetComponentsInParent<SkinnedMeshRenderer>() : m_targetSkins;

            // Grab the rig root from the first skin if it exists
            if (m_targetSkins != null && m_targetSkins.Length > 0)
                m_targetRigRoot = m_targetSkins[0].rootBone;
        }
    }
}