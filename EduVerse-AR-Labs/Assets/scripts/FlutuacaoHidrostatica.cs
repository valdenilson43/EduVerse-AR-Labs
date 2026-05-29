using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlutuacaoHidrostatica : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Configurações de Hidrostática")]
    public float densidadeLiquido = 1000f; // Água = 1000 kg/m³
    public float volumeObjeto = 0.005f;    // Volume em m³
    public float gravidade = 9.81f;
    public float resistenciaAgua = 3f;     // Amortecimento para o objeto não quicar para sempre

    private bool estaNaAgua = false;
    private float nivelDaAgua = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (estaNaAgua)
        {
            // 1. Calcula a profundidade do objeto em relação à superfície da água
            float profundidade = nivelDaAgua - transform.position.y;

            if (profundidade > 0)
            {
                // Para simplificar: calcula a porcentagem submersa aproximada (entre 0 e 1)
                float porcentagemSubmersa = Mathf.Clamp01(profundidade / transform.localScale.y + 0.5f);

                // 2. FÓRMULA DO EMPUXO: E = d * V * g
                float forcaEmpuxo = densidadeLiquido * (volumeObjeto * porcentagemSubmersa) * gravidade;

                // 3. Aplica a força de Empuxo para cima
                rb.AddForce(Vector3.up * forcaEmpuxo, ForceMode.Force);

                // 4. Aplica uma resistência (Arrasto) para o objeto estabilizar e flutuar suavemente
                rb.linearVelocity = Vector3.MoveTowards(rb.Velocity, Vector3.zero, resistenciaAgua * Time.fixedDeltaTime);
            }
        }
    }

    // Detecta quando o objeto entrou no "Trigger" da água
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agua"))
        {
            estaNaAgua = true;
            // Pega a altura máxima do topo do cubo de água (a superfície)
            nivelDaAgua = other.bounds.max.y;
        }
    }

    // Detecta se o objeto saiu completamente da água
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Agua"))
        {
            estaNaAgua = false;
        }
    }
}