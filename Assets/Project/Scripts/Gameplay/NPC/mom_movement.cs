using UnityEngine;
using UnityEngine.AI;

public class mom_movement : MonoBehaviour
{
    [Header("Ruta a seguir")]
    public Transform[] puntosDeRuta;
    
    [Header("Interacción")]
    public bool estaHablando = false;

    private int indiceActual = 0;
    private NavMeshAgent agente;
    private Animator animator;

    private Camera cam;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        cam = Camera.main; 

        agente.updateRotation = false; 

        IrAlSiguientePunto();
    }

    void Update()
    {
        if (estaHablando)
        {
            animator.SetBool("isMoving", false);
            return; 
        }

        ActualizarAnimacionesBaseEnPerspectiva();

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            IrAlSiguientePunto();
        }
    }

    
    void ActualizarAnimacionesBaseEnPerspectiva()
    {
        Vector3 velocidadWorld = agente.velocity;
        bool seEstaMoviendo = velocidadWorld.magnitude > 0.1f;
        
        animator.SetBool("isMoving", seEstaMoviendo);

        if (seEstaMoviendo)
        {
          
            Vector3 direccionWorld = velocidadWorld.normalized;

           
            Vector2 parametrosVisuales = CalcularParametrosRelativosALaCamara(direccionWorld);

     
            animator.SetFloat("Horizontal", parametrosVisuales.x);
            animator.SetFloat("Vertical", parametrosVisuales.y);
        }
    }

    
   
    private Vector2 CalcularParametrosRelativosALaCamara(Vector3 direccionMovementWorld)
    {
       
        Vector3 camForwardWorld = cam.transform.forward;
        Vector3 camRightWorld = cam.transform.right;


        camForwardWorld.y = 0;
        camRightWorld.y = 0;
        camForwardWorld.Normalize();
        camRightWorld.Normalize();

       
        float dotH = -Vector3.Dot(direccionMovementWorld, camRightWorld);
        
        float dotV = Vector3.Dot(direccionMovementWorld, camForwardWorld);

        return new Vector2(dotH, dotV);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            estaHablando = true;
            agente.isStopped = true;

            Vector3 direccionAlJugadorWorld = (other.transform.position - transform.position).normalized;
            Vector2 parametrosVisuales = CalcularParametrosRelativosALaCamara(direccionAlJugadorWorld);

            animator.SetFloat("Horizontal", parametrosVisuales.x);
            animator.SetFloat("Vertical", parametrosVisuales.y);
        }
    }

    void IrAlSiguientePunto()
    {
        if (puntosDeRuta.Length == 0) return;
        agente.destination = puntosDeRuta[indiceActual].position;
        indiceActual = (indiceActual + 1) % puntosDeRuta.Length;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            estaHablando = false;
            agente.isStopped = false;
        }
    }
}