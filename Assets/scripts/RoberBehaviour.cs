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

    public enum ActionState { IDLE, WORKING };
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int money = 800;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
        Leaf goToBackdoor = new Leaf("Go To Backdoor", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go To Frontdoor", GoToFrontdoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        Selector openDoor = new Selector("Open Door");
        openDoor.AddChild(goToFrontdoor);
        openDoor.AddChild(goToBackdoor);

        steal.AddChild(hasGotMoney);
        steal.AddChild(openDoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();
    }

    public Node.Status HasMoney() 
    {
        // Corrigido para verificar se há dinheiro suficiente
        if (money <= 500) return Node.Status.SUCCESS; 
        return Node.Status.FAILURE; 
    }

    public Node.Status GoToDiamond() 
    {
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS) 
        {
            diamond.transform.parent = this.gameObject.transform; // Anexa o diamante ao Rober
        }
        return s;
    }

    public Node.Status GoToBackdoor() 
    {
        return GoToDoor(backdoor);
    }

    public Node.Status GoToFrontdoor() 
    {
        return GoToDoor(frontdoor);
    }

    public Node.Status GoToVan() 
    {
        return GoToLocation(van.transform.position);
    }

    public Node.Status GoToDoor(GameObject door) 
    {
        Node.Status s = GoToLocation(door.transform.position); 
        if (s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked) // Verifica se a porta está trancada
            {
                door.SetActive(false); // Abre a porta
                return Node.Status.SUCCESS; 
            }
            return Node.Status.FAILURE; // A porta está trancada
        }
        return s; 
    }

    Node.Status GoToLocation(Vector3 destination) 
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE) 
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        } 
        else if (distanceToTarget < 2.0f) 
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS; // Chegou ao destino
        } 
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2.0f) 
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE; // Não chegou ao destino
        }
        return Node.Status.RUNNING; // A caminho
    }

    // Update é chamado uma vez por frame
    void Update()
    {
        if(treeStatus != Node.Status.SUCCESS)
            treeStatus = tree.Process(); // Processa a árvore de comportamento
    }
}
