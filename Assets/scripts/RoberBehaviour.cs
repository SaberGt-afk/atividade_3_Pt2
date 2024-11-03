using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoberBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    public GameObject diamond;
    public GameObject van;
    NavMeshAgent agent;
    public GameObject backdoor;
    public GameObject frontdoor;
    
    public enum ActionState { IDLE, WORKING};
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;
    
    // Start is called before the first frame update
    void Start()
    {

        agent = GetComponent<NavMeshAgent>();


        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToBackdoor = new Leaf("Go To Backdoor", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go To Frontdoor", GoToFrontdoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        Selector opendoor = new Selector("Open Door");

        opendoor.AddChild(goToFrontdoor);
        opendoor.AddChild(goToBackdoor);
        

        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
        //steal.AddChild(goToBackdoor);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();
    }

    

    public Node.Status GoToDiamond() 
    {
        return GoToLocation(diamond.transform.position);
    }
    public Node.Status GoToVan() 
    {
        return GoToLocation(van.transform.position);
    }
    public Node.Status GoToBackdoor() 
    {
        return GoToLocation(backdoor.transform.position);
    }

    public Node.Status GoToFrontdoor() 
    {
         return GoToLocation(frontdoor.transform.position);
    }

     Node.Status GoToLocation(Vector3 destination) 
     {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE) 
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        } 
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2.0f) 
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        } 
        else if (distanceToTarget < 2.0f) 
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }
    // Update is called once per frame
    void Update()
    {
        if (treeStatus == Node.Status.RUNNING)
            treeStatus = tree.Process();
    }
}
