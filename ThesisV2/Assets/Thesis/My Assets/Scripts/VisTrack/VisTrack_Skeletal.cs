using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using Thesis.Interface;
using Thesis.Visualization;

namespace Thesis.VisTrack
{
    [RequireComponent(typeof(Animator))]
    public class VisTrack_Skeletal : MonoBehaviour, IVisualizable
    {
        //--- Data Struct ---//
        /*public struct Data_Renderables
        {
            public Data_Renderables(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                m_timestamp = float.Parse(tokens[0]);

#if UNITY_EDITOR
                // The second token is the path to the mesh and its submesh ID (-1 if not a submesh)
                var meshTokens = tokens[1].Split(',');
                string meshPath = meshTokens[0];
                int subMeshIndex = int.Parse(meshTokens[1]);

                // If not a submesh, just load the mesh directly
                // If it is a submesh, we need to load all of the assets at the path and grab the one from the right index
                if (subMeshIndex == -1)
                {
                    m_mesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh)) as Mesh;
                }
                else
                {
                    var subAssets = AssetDatabase.LoadAllAssetsAtPath(meshPath);
                    m_mesh = subAssets[subMeshIndex] as Mesh;
                }

                // The third token is the colour which is a vector3 so parse that
                m_color = Utility_Functions.ParseColor(tokens[2]);

                // Remaining tokens are the materials so load them too
                m_materials = new List<Material>();
                for (int i = 3; i < tokens.Length; i++)
                    m_materials.Add(AssetDatabase.LoadAssetAtPath(tokens[i], typeof(Material)) as Material);

#else
                // Convert the mesh path to a resources based path instead of an asset path
                string meshResourcePath = Utility_Functions.ConvertAssetToResourcePath(meshPath);

                // If not a submesh, just load the mesh directly
                // If it is a submesh, we need to load all of the assets at the path and grab the one from the right index
                if (subMeshIndex == -1)
                {
                    m_mesh = Resources.Load(meshResourcePath, typeof(Mesh)) as Mesh;
                }
                else
                {
                    var subAssets = Resources.LoadAll(meshPath);
                    m_mesh = subAssets[subMeshIndex] as Mesh;
                }
                
                // Convert all of the materials and load them in as well
                for (int i = 3; i < tokens.Length; i++)
                {
                    string matResourcePath = Utility_Functions.ConvertAssetToResourcePath(tokens[i]);
                    m_materials.Add(Resources.Load(matResourcePath, typeof(Material)) as Material);
                }
#endif
            }

            public static List<Data_Renderables> ParseDataList(string _data)
            {
                // Create a list to hold the parsed data
                List<Data_Renderables> dataPoints = new List<Data_Renderables>();

                // Split the string into individual lines which each are one data point
                string[] lines = _data.Split('\n');

                // Create new data points from each of the lines
                foreach (string line in lines)
                {
                    // If the line is empty, do nothing
                    if (line == null || line == "")
                        continue;

                    // Otherwise, create a new data point
                    dataPoints.Add(new Data_Renderables(line));
                }

                // Return the list of data points
                return dataPoints;
            }

            public float m_timestamp;
            public Mesh m_mesh;
            //public Material m_material;
            public List<Material> m_materials;
            public Color m_color;
        }
        */

        public struct Data_Skeletal_Anim
        {
            public Data_Skeletal_Anim(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                this.m_timestamp = float.Parse(tokens[0]);

                // The second token is the animation name
                this.m_animName = tokens[1];

                // The final token is the normalized animation time so just parse the float
                this.m_animTime = float.Parse(tokens[2]);
            }

            public static List<Data_Skeletal_Anim> ParseDataList(string[] _lines)
            {
                // Create a list to hold the data points
                List<Data_Skeletal_Anim> dataPoints = new List<Data_Skeletal_Anim>();

                // Create new data points from each of the lines
                foreach (string line in _lines)
                {
                    // If the line is empty, do nothing
                    if (line == null || line == "")
                        continue;

                    // Otherwise, create a new data point
                    dataPoints.Add(new Data_Skeletal_Anim(line));
                }

                // Return the list of data points
                return dataPoints;
            }

            public float m_timestamp;
            public string m_animName;
            public float m_animTime;
        }

        //--- Private Variables ---//
        private Animator m_targetAnimator;
        private List<Data_Skeletal_Anim> m_animDataPoints;
        private List<Transform> m_boneObjs;



        //--- IVisualizable Interface ---// 
        public bool InitWithString(string _data)
        {
            try
            {
                // The first line is always the skeleton information so we should use it create the skeleton and then remove it from the list
                string[] dataLines = _data.Split('\n');
                string skeletalInfo = dataLines[0];
                dataLines = dataLines.Skip(1).ToArray();

                // Initialize the skeleton and animator together
                m_targetAnimator = GetComponent<Animator>();
                InitializeSkeleton(m_targetAnimator, skeletalInfo);

                // Create a list of animation data points by parsing the rest of string
                m_animDataPoints = Data_Skeletal_Anim.ParseDataList(dataLines);

                // If everything worked correctly, return true
                return true;
            }
            catch (Exception _e)
            {
                // If something went wrong, output an error and return false
                Debug.LogError("Error in InitWithString(): " + _e.Message);
                return false;
            }
        }

        public void StartVisualization(float _startTime)
        {
            // Stop the animator from moving through its animations naturally
            m_targetAnimator.speed = 0.0f;

            // Apply the initial visualization
            UpdateVisualization(_startTime);

            // Only configure the mesh collider if this object is a key object
            if (GetComponent<Visualization_Object>().IsKeyObj)
            {
                // TODO: Add mesh colliders for the skinned meshes!
                // ...

                //// Setup the mesh collider so it is ready for mouse picking and uses the correct mesh
                //// Use all of the cooking options
                //MeshCollider meshCollider = this.GetComponent<MeshCollider>();
                //meshCollider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.UseFastMidphase | MeshColliderCookingOptions.WeldColocatedVertices;
                //meshCollider.sharedMesh = m_targetFilter.sharedMesh;
                //meshCollider.convex = false;

                //// We need to set the object to be on the focus picking layer so that we can focus it with the camera controls
                //// NOTE: This feature REQUIRES that this layer is added and is exactly this
                //this.gameObject.layer = LayerMask.NameToLayer("FocusTargetPicking");
            }
        }

        public void UpdateVisualization(float _time)
        {
            // Create a variable to hold the interpolated data 
            Data_Skeletal_Anim finalDataPoint;

            // Get the data point before the given time and the index for the datapoint after
            int prevDataIdx = FindDataPointForTime(_time);
            int nextDataIdx = prevDataIdx + 1;
            Data_Skeletal_Anim prevDataPoint = m_animDataPoints[prevDataIdx];

            // Set the result data to be the data point before the time by default
            finalDataPoint = prevDataPoint;

            // If the datapoint after exists, possible interpolate between them
            if (nextDataIdx < m_animDataPoints.Count)
            {
                // Grab the next data point
                Data_Skeletal_Anim nextDataPoint = m_animDataPoints[nextDataIdx];

                // If the two data points represent different animations, then do nothing and stay defaulted to the first one
                // Otherwise, lerp the animation between them
                if (prevDataPoint.m_animName == nextDataPoint.m_animName)
                {
                    // Calculate the lerp T param between the before and after points
                    float lerpT = Mathf.InverseLerp(prevDataPoint.m_timestamp, nextDataPoint.m_timestamp, _time);

                    // Set the final data's animation time to be a lerp'd value between the two points
                    finalDataPoint.m_animTime = Mathf.Lerp(prevDataPoint.m_animTime, nextDataPoint.m_animTime, lerpT);
                }
            }

            // Apply the data point to the visualization
            // TODO: Support multiple animation layers
            m_targetAnimator.Play(finalDataPoint.m_animName, 0, finalDataPoint.m_animTime);
    }

        public int FindDataPointForTime(float _time)
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_animDataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_animDataPoints.Count >= 1, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Start by setting the selected index to 0 in case there is only one point
            int selectedIndex = 0;

            // Loop through all of the data and find the nearest point BEFORE the given time
            for (selectedIndex = 0; selectedIndex < m_animDataPoints.Count - 1; selectedIndex++)
            {
                // Get the datapoint at the current index and next index
                var thisDataPoint = m_animDataPoints[selectedIndex];
                var nextDataPoint = m_animDataPoints[selectedIndex + 1];

                // If this datapoint is BEFORE OR AT the time and the next one is AFTER the time, then we are at the right data point
                if (thisDataPoint.m_timestamp <= _time && nextDataPoint.m_timestamp > _time)
                    break;
            }

            // Return the selected index
            return selectedIndex;
        }

        public string GetTrackName()
        {
            return "Renderables";
        }

        public float GetFirstTimestamp()
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_animDataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_animDataPoints.Count >= 1, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Return the timestamp for the first data point
            return m_animDataPoints[0].m_timestamp;
        }

        public float GetLastTimestamp()
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_animDataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_animDataPoints.Count >= 1, "Track Assert Failed [" + GetTrackName() + "] - " + "m_animDataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Return the timestamp for the last data point
            return m_animDataPoints[m_animDataPoints.Count - 1].m_timestamp;
        }



        //--- Utility Methods ---//
        private void InitializeSkeleton(Animator _animator, string _skeletalInfo)
        {
            // Split the skeletal info into its major components
            string[] skeletalInfoTokens = _skeletalInfo.Split('~');

            // The first major token is information for the animator controller
            HandleAnimatorController(_animator, skeletalInfoTokens[0]);

            // The second major token is the rig information 
            HandleRigCreation(skeletalInfoTokens[1]);

            // The third major token is the skinned mesh information
            HandleSkinnedMeshCreation(skeletalInfoTokens[2]);
        }

        private void HandleAnimatorController(Animator _animator, string _animatorString)
        {
            // Load the animator controller in from the assets and pass it to the animator
            // TODO: Make this work with resources as well!
            RuntimeAnimatorController animatorController = AssetDatabase.LoadAssetAtPath(_animatorString, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
            _animator.runtimeAnimatorController = animatorController;
        }

        private void HandleRigCreation(string _rigString)
        {
            // Create lists to store the names and parent indices for each bone
            List<string> boneNames = new List<string>();
            List<int> boneParentIndices = new List<int>();

            // Split the string to get all of the individual bone tokens
            string[] boneTokenStrs = _rigString.Split(',');

            // Loop through and extract all of the names and parent indices for the bones
            foreach(var boneToken in boneTokenStrs)
            {
                // If the token is empty, just skip it (happens at the end of the list)
                if (boneToken == "" || boneToken == " ")
                    continue;

                // The bone token can be split further
                string[] boneInfoTokens = boneToken.Split('`');

                // The first result is the name of the bone so add it to the list
                boneNames.Add(boneInfoTokens[0]);

                // The second is the index of the parent so parse it and then add it to the list
                boneParentIndices.Add(int.Parse(boneInfoTokens[1]));
            }

            // Ensure both the names and indices arrays match in length
            Assert.IsTrue(boneNames.Count == boneParentIndices.Count, "Track Assert Failed [" + GetTrackName() + "] - " + "The number of parsed rig bone names and rig parent indices needs to be equal");

            // Now that we have all the bone names, we can go ahead and create all of the actual bone objects in the scene
            m_boneObjs = new List<Transform>();
            foreach (var boneName in boneNames)
            {
                // Instantiate a new object with the correct name
                GameObject newBoneObj = new GameObject(boneName);

                // Store the bone in the list
                m_boneObjs.Add(newBoneObj.transform);
            }

            // Ensure the correct number of bones have been created
            Assert.IsTrue(m_boneObjs.Count == boneNames.Count, "Track Assert Failed [" + GetTrackName() + "] - " + "The number of generated bone objects and the number of parsed bone names needs to be equal");

            // Now that the bones are made, we can link them up to create the proper hierarchy
            for (int i = 0; i < m_boneObjs.Count; i++)
            {
                // Get this bone's parent index
                int parentIndex = boneParentIndices[i];

                // If the index is -1, it is the root bone and so can just be a child of the main object
                // Otherwise, the index refers to another bone in the rig and so we can link it to that one
                m_boneObjs[i].parent = (parentIndex == -1) ? this.transform : m_boneObjs[parentIndex];
            }
        }

        private void HandleSkinnedMeshCreation(string _skinnedString)
        {
            // Split the skinned string to get information for each of the individiual skinned mesh renderers
            string[] rendererStrs = _skinnedString.Split(',');

            // Setup each of the invidual skinned mesh renderers
            foreach(string rendererStr in rendererStrs)
            {
                // Skip empty string
                if (rendererStr == "" || rendererStr == " ")
                    continue;

                // Split the string again to separate the meshes and material information
                string[] splitRendererStrs = rendererStr.Split(';');

                // Create a new skinned mesh renderer object as a child of this object
                GameObject skinnedMeshObj = new GameObject("Skinned Mesh");
                skinnedMeshObj.transform.parent = this.transform;
                SkinnedMeshRenderer skinnedMeshComp = skinnedMeshObj.AddComponent<SkinnedMeshRenderer>();

                // The first major token deals with the mesh object
                HandleSkinnedMesh_MeshLoading(skinnedMeshComp, splitRendererStrs[0]);

                // The second major token deals with the materials
                HandleSkinnedMesh_MatLoading(skinnedMeshComp, splitRendererStrs[1]);

                // The third major token deals with the joints
                HandleSkinnedMesh_JointLoading(skinnedMeshComp, splitRendererStrs[2]);
            }
        }

        private void HandleSkinnedMesh_MeshLoading(SkinnedMeshRenderer _meshComp, string _meshString)
        {
            // Split the mesh string into the tokens
            string[] meshTokens = _meshString.Split('`');

            // The first token is the path to the mesh asset
            string meshPath = meshTokens[0];

            // The second token is the mesh index, if the mesh is a subasset
            int subMeshIndex = int.Parse(meshTokens[1]);

            // If not a submesh, just load the mesh directly and give it to the skinned renderer
            // If it is a submesh, we need to load all of the assets at the path and grab the one from the right index
            if (subMeshIndex == -1)
            {
                _meshComp.sharedMesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh)) as Mesh;
            }
            else
            {
                var subAssets = AssetDatabase.LoadAllAssetsAtPath(meshPath);
                _meshComp.sharedMesh = subAssets[subMeshIndex] as Mesh;
            }
        }

        private void HandleSkinnedMesh_MatLoading(SkinnedMeshRenderer _meshComp, string _matString)
        {
            // Split the material string into the tokens
            string[] matTokens = _matString.Split('`');

            // Prepare to hold all of the loaded materials
            List<Material> loadedMats = new List<Material>();

            // Load all of the materials
            foreach(var matPath in matTokens)
            {
                // Skip empty ones
                if (matPath == "" || matPath == " ")
                    continue;

                // Load the material
                // TODO: Support using resources as well!
                // TODO: Support loading materials as subassets from FBX's
                Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

                // If the material exists, add it to the list
                if (mat != null)
                    loadedMats.Add(mat);
            }

            // Pass the materials to the mesh renderer
            _meshComp.sharedMaterials = loadedMats.ToArray();
        }

        private void HandleSkinnedMesh_JointLoading(SkinnedMeshRenderer _meshComp, string _jointString)
        {
            // Split the string to get the individual joint indices as strings
            string[] jointTokens = _jointString.Split('`');

            // Create a list to hold all of the joints for this skinned mesh
            List<Transform> jointList = new List<Transform>();

            // Go through all of the parsed indices and find the matching joint
            foreach(var jointToken in jointTokens)
            {
                // Skip empty ones
                if (jointToken == "" || jointToken == " ")
                    continue;

                // Parse the index
                int jointIndex = int.Parse(jointToken);

                // Find the bone that matches this index
                Transform matchingBone = m_boneObjs[jointIndex];

                // Add the bone to the list
                jointList.Add(matchingBone);
            }

            // Pass the list of joints to the skinned mesh renderer so it knows what to draw on
            _meshComp.bones = jointList.ToArray();
        }
    }
}