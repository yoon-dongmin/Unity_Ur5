using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.PandaMove;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
// using System.DateTime;
using System.Windows;

public class InitSync : MonoBehaviour
{
    private ROSConnection ros;
    // ros service name
    private string rosServiceName = "init_sync"; 
    public GameObject ur5;
    public static ArticulationBody[] jointArticulationBodies = new ArticulationBody[9];

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.instance;

        // Set Panda Gameobject Variables
        ur5 = GameObject.Find("ur5");

        string rail_0 = "world/rail0";
        string rail_1 = rail_0 + "/rail1";
        jointArticulationBodies[0] = ur5.transform.Find(rail_1).GetComponent<ArticulationBody>();

        string base_link = rail_1 + "/base_link";
        string shoulder_link = base_link + "/shoulder_link";
        jointArticulationBodies[1] = ur5.transform.Find(shoulder_link).GetComponent<ArticulationBody>();

        string upper_arm_link = shoulder_link + "/upper_arm_link";
        jointArticulationBodies[2] = ur5.transform.Find(upper_arm_link).GetComponent<ArticulationBody>();

        string forearm_link = upper_arm_link + "/forearm_link";
        jointArticulationBodies[3] = ur5.transform.Find(forearm_link).GetComponent<ArticulationBody>();

        string wrist_1_link = forearm_link + "/wrist_1_link";
        jointArticulationBodies[4] = ur5.transform.Find(wrist_1_link).GetComponent<ArticulationBody>();

        string wrist_2_link = wrist_1_link + "/wrist_2_link";
        jointArticulationBodies[5] = ur5.transform.Find(wrist_2_link).GetComponent<ArticulationBody>();

        string wrist_3_link = wrist_2_link + "/wrist_3_link";
        jointArticulationBodies[6] = ur5.transform.Find(wrist_3_link).GetComponent<ArticulationBody>();

        string panda_hand = wrist_3_link + "/tool0"+ "/panda_hand";
        string panda_rightfinger = panda_hand + "/panda_rightfinger";
        jointArticulationBodies[7] = ur5.transform.Find(panda_rightfinger).GetComponent<ArticulationBody>();

        string panda_leftfinger = panda_hand + "/panda_leftfinger";
        jointArticulationBodies[8] = ur5.transform.Find(panda_leftfinger).GetComponent<ArticulationBody>();

        // Get ROS Scene Configuration by ROS Service

        ros.ImplementService<MInitRequest>(rosServiceName, GetScenePose);

        Debug.Log("init_sync server ON");
    }

    private MInitResponse GetScenePose(MInitRequest request)
    {
        Debug.Log("Received Initial Scene Configuration");
        MInitResponse response = new MInitResponse();
        try
        {
            ur5 = GameObject.Find("ur5");
            var rail1XDrive = jointArticulationBodies[0].xDrive;
            // rail1XDrive.stiffness = (float) 100000;
            rail1XDrive.stiffness = (float) 90000;
            ur5.transform.Find("world/rail0/rail1").GetComponent<ArticulationBody>().xDrive = rail1XDrive;

            ExecuteTrajectories(request.joint_state);
            // ObjectDeactivate(request.deactivated_object);
            StartCoroutine(ObjectSync(request));

            Debug.Log("Initial Scen Syncing Completed");

            response.init_state = 1;

            return response;
        }
        catch (Exception ex)
        {
            Debug.Log("Initial Scene Syncing Error Occured");
            Debug.Log(ex);
            response.init_state = 0;
            return response;
        }
    }

    void ExecuteTrajectories(double[] jointStates)
    {
        float[] convertedPositions = jointStates.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

        var joint1XDriveRail = jointArticulationBodies[0].xDrive;
        joint1XDriveRail.target = (float)jointStates[0];
        jointArticulationBodies[0].xDrive = joint1XDriveRail;

        for (int jointIndex = 1; jointIndex < jointStates.Length; jointIndex++)
        {
            var joint1XDrive = jointArticulationBodies[jointIndex].xDrive;
            joint1XDrive.target = convertedPositions[jointIndex];
            jointArticulationBodies[jointIndex].xDrive = joint1XDrive;
        }
    }

    private IEnumerator ObjectSync(MInitRequest request)
    {
        for (int objectIndex = 0; objectIndex < request.activated_object.Length; objectIndex++)
        {
            // GameObject targetObject = GameObject.Find(request.object_name[objectIndex]);
            GameObject targetObject = GameObject.Find("Plane").transform.Find(request.activated_object[objectIndex]).gameObject;
            // GameObject.Find("Plane").transform.Find(request.object_name[objectIndex]).gameObject;
            targetObject.transform.position = request.position[objectIndex].From<FLU>();
            // targetObject.transform.rotation = request.orientation[objectIndex].From<FLU>();
            // targetObject.transform.rotation = request.orientation[objectIndex].Quaternion<c>;
            targetObject.transform.rotation = request.orientation[objectIndex].From<FLU>() * Quaternion.Euler(0, 90, 0);
            // targetObject.transform.Rotate(Euler.Quaternion(request.orientation[objectIndex].From<FLU>()));
            // targetObject.transform.localScale = request.scale[objectIndex].From<FLU>();
            targetObject.transform.localScale = new Vector3((float)request.scale[objectIndex].y, (float)request.scale[objectIndex].z, (float)request.scale[objectIndex].x);
            targetObject.SetActive(true);
            Debug.Log(request.activated_object[objectIndex] + " Sync Completed");

            yield return new WaitForSeconds(0.3f);
        }
        print("init Sync Completed");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
