using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

public class EventosManager : MonoBehaviour//trata da geração de eventos
{
    public ListaEventos ListaEventos;

    public static EventosManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    public EventosSO BuscaEvento()//retorna um evento valido
    {
        int chanceEvento = 25;//25% de chance por turno de ocorrer evento
        if (UnityEngine.Random.Range(1, 101) > chanceEvento) //caso não haja evento
        {
            return null;
        }

        if (ListaEventos == null) Debug.Log("ListaEventos");
        if (ListaEventos.Eventos == null) Debug.Log("Eventos");
        var EventoEscolhido = ListaEventos.Eventos[Random.Range(0, ListaEventos.Eventos.Count)];

        var ListaEventosValidos = new List<EventosSO>();

        //verifiva quais eventos tem triggers ativos
        if (TriggerAtivo(EventoEscolhido)) {
            ListaEventosValidos.Add(EventoEscolhido);
        }

        if (ListaEventosValidos.Count == 0) return null;//caso não haja eventos validos

        //Retorna um evento aleatório
        return ListaEventosValidos[Random.Range(0, ListaEventosValidos.Count)];
    }

    private bool TriggerAtivo(EventosSO evento) //verifica se um trigger é true ou false
    {
        var trigger = evento.TriggerID;

        var DicionarioTrigger = TriggerManager.Instance.Triggers;

        if (!DicionarioTrigger.ContainsKey(trigger)) return false;

        var funcaoTrigger = DicionarioTrigger[trigger]; //busca a funçao do trigger

        return funcaoTrigger.Invoke();//retorna o resultado dela
    }
}
