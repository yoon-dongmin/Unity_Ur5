using System.Collections;
using System.Linq;
using RosMessageTypes.PandaMoveit;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

public class TrajectoryTest : MonoBehaviour
{
    private ROSConnection ros;
    private int numRobotJoints = 7;
    private readonly float jointAssignmentWait = 0.1f;
    public string rosServiceName = "panda_moveit";
    public GameObject ur5;
    private ArticulationBody[] jointArticulationBodies;

    /// <returns>PandaMoveitJoints</returns>

    public void PublishJoints()
    {
        MTestRequest request = new MTestRequest();
        request.test_num = 1;

        ros.SendServiceMessage<MTestResponse>(rosServiceName, request, TrajectoryResponse);
    }

    void TrajectoryResponse(MTestResponse response)
    {
        if (response.trajectories.Length > 0)
        {
            Debug.Log("Trajectory returned.");
            StartCoroutine(ExecuteTrajectories(response));
        }
        else
        {
            Debug.LogError("No trajectory returned from TestService");
        }
    }

    /// <param name="response"> </param>
    /// <returns></returns>

    private IEnumerator ExecuteTrajectories(MTestResponse response)
    {
        if (response.trajectories != null)
        {
            for (int poseIndex = 0; poseIndex < response.trajectories.Length; poseIndex++)
            {
                for (int jointConfigIndex = 0; jointConfigIndex < response.trajectories[poseIndex].joint_trajectory.points.Length; jointConfigIndex++)
                {
                    var jointPositions = response.trajectories[poseIndex].joint_trajectory.points[jointConfigIndex].positions;
                    float[] result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

                    for (int joint = 0; joint < jointArticulationBodies.Length; joint++)
                    {
                        var joint1XDrive = jointArticulationBodies[joint].xDrive;
                        joint1XDrive.target = result[joint];
                        jointArticulationBodies[joint].xDrive = joint1XDrive;
                    }

                    yield return new WaitForSeconds(jointAssignmentWait);
                }
            }
        }
    }

    void Start()
    {
        ros = ROSConnection.instance;

        jointArticulationBodies = new ArticulationBody[numRobotJoints];

        string rail_0 = "world/rail0";
        // jointArticulationBodies[0] = ur5.transform.Find(rail_0).GetComponent<ArticulationBody>();

        string rail_1 = rail_0 + "/rail1";
        jointArticulationBodies[0] = ur5.transform.Find(rail_1).GetComponent<ArticulationBody>();

        string base_link = rail_1 + "/base_link";
        // jointArticulationBodies[2] = ur5.transform.Find(base_link).GetComponent<ArticulationBody>();

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


        // string panda_link_8 = panda_link_7 + "/panda_link8";
        // jointArticulationBodies[10] = panda.transform.Find(panda_link_8).GetComponent<ArticulationBody>();

        // string panda_link_ee_ = panda_link_8 + "/panda_link_ee";
        // jointArticulationBodies[11] = panda.transform.Find(panda_link_ee_).GetComponent<ArticulationBody>();

    }
}
