using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour//contem todos o triggers, ou seja condições que definem quando um evento aleatorio pode se chamado
{

    public Dictionary<string, Func<bool>> Triggers;

    public static TriggerManager Instance;

    private void Awake()
    {
        Instance = this;
        Triggers = new Dictionary<string, Func<bool>>
        {
            {"sempre", () => true },

            //esta lesionado
            {"isLesionado", () => PlayerManager.Instance.Player.MesRepouso > 0 },

            {"isLesionado&IdadeMais33", () => PlayerManager.Instance.Player.MesRepouso > 0 && PlayerManager.Instance.Player.Idade >= 32 },

            //idade e overall
            {"idadeMenos22&RepMais600", () => PlayerManager.Instance.Player.Idade <= 22 && PlayerManager.Instance.Player.Reputacao >= 600 },

            //idade
            {"idadeMais32", () => PlayerManager.Instance.Player.Idade >= 32 },

            {"IdadeMenos20", () => PlayerManager.Instance.Player.Idade <= 20  },

            {"IdadeMenos25", () => PlayerManager.Instance.Player.Idade <= 25  },


            //inteligencia
            {"InteligenciaMenos30", () => PlayerManager.Instance.Player.JAtributos.Inteligencia <= 30  },

            {"InteligenciaMenos70", () => PlayerManager.Instance.Player.JAtributos.Inteligencia <= 70  },

            {"InteligenciaMais70", () => PlayerManager.Instance.Player.JAtributos.Inteligencia >= 70  },


            //Ivestimento
            {"InvestimentoMais70", () => PlayerManager.Instance.Player.JAtributos.Investimento >= 70  },

            {"InvestimentoMenos30", () => PlayerManager.Instance.Player.JAtributos.Inteligencia <=30  },

            //forca
            {"ForcaMais70", () => PlayerManager.Instance.Player.JAtributos.Forca >= 70  },

        };
    }
}
