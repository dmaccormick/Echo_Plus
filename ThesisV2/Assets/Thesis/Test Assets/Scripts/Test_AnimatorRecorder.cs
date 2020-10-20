using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Test_AnimatorRecorder : MonoBehaviour
{
    class Test_AnimatorRecorderData
    {
        public Test_AnimatorRecorderData(string _animName, float _animTime)
        {
            this.m_animName = _animName;
            this.m_animTime = _animTime;
        }

        public string m_animName;
        public float m_animTime;
    }

    public Transform m_rigRoot;
    public SkinnedMeshRenderer[] m_skinnedMeshes;
    public Animator m_animator;

    private Animator m_copiedAnimator;
    private bool m_isRecording;
    private List<Test_AnimatorRecorderData> m_data;
    private int m_currentPlaybackIdx;

    private List<Transform> allOriginalBones;
    private List<Transform> copiedBones;



    // Start is called before the first frame update
    void Start()
    {
        m_isRecording = true;
        m_data = new List<Test_AnimatorRecorderData>();
        m_currentPlaybackIdx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    m_isRecording = !m_isRecording;

        //    if (m_isRecording)
        //    {
        //        m_data.Clear();
        //        m_animator.speed = 1.0f;
        //    }
        //    else
        //    {
        //        m_currentPlaybackIdx = 0;
        //        m_animator.speed = 0.0f;
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.L))
        {
            var newObject = new GameObject("Copied Object");

            DuplicateRig(newObject.transform);

            m_copiedAnimator = newObject.AddComponent<Animator>();
            m_copiedAnimator.runtimeAnimatorController = m_animator.runtimeAnimatorController;

            foreach(var skinnedMesh in m_skinnedMeshes)
            {
                var newSkinnedMeshObj = new GameObject("Skinned Mesh");
                newSkinnedMeshObj.transform.parent = newObject.transform;

                var newSkinnedMesh = newSkinnedMeshObj.AddComponent<SkinnedMeshRenderer>();
                newSkinnedMesh.sharedMesh = skinnedMesh.sharedMesh;
                newSkinnedMesh.localBounds = skinnedMesh.localBounds;
                newSkinnedMesh.sharedMaterials = skinnedMesh.sharedMaterials;

                // Use the new rig's matching root bone 
                int rootBoneIndex = allOriginalBones.IndexOf(skinnedMesh.rootBone);
                //newSkinnedMesh.rootBone = copiedBones[rootBoneIndex];

                // Set the actual bones to match as well
                SetMatchingBones(skinnedMesh, newSkinnedMesh);
            }

            m_isRecording = false;
            m_animator.speed = 0.0f;
        }

        if (m_isRecording)
        {
            var clipInfo = m_animator.GetCurrentAnimatorClipInfo(0)[0];
            var stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

            m_data.Add(new Test_AnimatorRecorderData(clipInfo.clip.name, stateInfo.normalizedTime));
        }
        else
        {
            if (m_currentPlaybackIdx >= m_data.Count)
                m_currentPlaybackIdx = 0;

            var dataPoint = m_data[m_currentPlaybackIdx];
            //m_animator.Play(dataPoint.m_animName, 0, dataPoint.m_animTime);
            m_copiedAnimator.Play(dataPoint.m_animName, 0, dataPoint.m_animTime);

            m_currentPlaybackIdx++;
        }

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    m_stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        //    m_clipInfo = m_animator.GetCurrentAnimatorClipInfo(0)[0];
        //}

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    m_animator.Play(m_clipInfo.clip.name, 0, m_stateInfo.normalizedTime);
        //    m_animator.speed = 0.0f;
        //}
    }

    private void DuplicateRig(Transform _newParent)
    {
        // Get all of the bones in the rig
        Transform[] childBones = m_rigRoot.GetComponentsInChildren<Transform>();
        /*List<Transform> */allOriginalBones = new List<Transform>();
        //allOriginalBones.Add(m_rigRoot);
        allOriginalBones.AddRange(childBones);

        // Make a copy of the all of the bones
        /*List<Transform>*/ copiedBones = new List<Transform>();
        foreach(var boneToCopy in allOriginalBones)
        {
            var newBoneObj = new GameObject(boneToCopy.name);
            newBoneObj.transform.parent = _newParent;
            copiedBones.Add(newBoneObj.transform);
        }

        // Adjust the hierarchy so it matches the original
        for (int i = 0; i < copiedBones.Count; i++)
        {
            var originalBone = allOriginalBones[i];
            var copiedBone = copiedBones[i];

            // Determine the index of the original bone's parent
            if (allOriginalBones.Contains(originalBone.parent))
            {
                int parentIdx = allOriginalBones.IndexOf(originalBone.parent);

                // Now use the index to make the same parent / child relationship in the copied bones
                copiedBone.parent = copiedBones[parentIdx];
            }
        }

        // Set the starting transform information for the new bones
        for (int i = 0; i < copiedBones.Count; i++)
        {
            var originalBone = allOriginalBones[i];
            var copiedBone = copiedBones[i];

            Debug.Log("Ignoring default local transforms!");
            //copiedBone.localPosition = originalBone.localPosition;
            //copiedBone.localRotation = originalBone.localRotation;
            //copiedBone.localScale = originalBone.localScale;
        }
    }

    private void CreateBones(Transform _parent, SkinnedMeshRenderer _skinned)
    {
        List<Transform> newBones = new List<Transform>();

        foreach(var bone in _skinned.bones)
        {
            var newBoneObj = new GameObject(bone.name);
            newBoneObj.transform.parent = _parent;
            newBones.Add(newBoneObj.transform);
        }

        _skinned.bones = newBones.ToArray();
    }

    private void SetMatchingBones(SkinnedMeshRenderer _originalSkinned, SkinnedMeshRenderer _newSkinned)
    {
        // Get the indices for all of the original skinned mesh renderer's bones
        List<int> jointIndices = new List<int>();
        foreach (var originalJoint in _originalSkinned.bones)
            jointIndices.Add(allOriginalBones.IndexOf(originalJoint));

        // Use the indices to create a duplicate list of bones in the newly created set
        List<Transform> newJointList = new List<Transform>();
        foreach (var jointIndex in jointIndices)
            newJointList.Add(copiedBones[jointIndex]);

        _newSkinned.bones = newJointList.ToArray();
    }
}
