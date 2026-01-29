using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarrasManager : MonoBehaviour//gere as barras de cansaço e aprovação
{
    public Slider BarraCansaco;
    public Slider BarraCansacoMenu;
    public Slider BarraAprovMenu;
    public Slider BarraAprovCarreira;
    public static BarrasManager Instance;


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void Start()
    {
        BarraCansaco.value = PlayerManager.Instance.Player.Cansaco;
        BarraCansacoMenu.value = PlayerManager.Instance.Player.Cansaco;
        BarraAprovMenu.value = PlayerManager.Instance.Player.AprovTreinador;
    }
    public void AtualizaBarras()//atualiza todas as barras
    {
        var Player = PlayerManager.Instance.Player;

        //anima barras de cansaço
        AnimaBarra(BarraCansaco, Player.Cansaco);
        AnimaBarra(BarraCansacoMenu, Player.Cansaco);

        //anima barras de aprovação do treinador
        AnimaBarra(BarraAprovMenu, (int)Player.AprovTreinador);
        AnimaBarra(BarraAprovCarreira, (int)Player.AprovTreinador);

    }

    public void AnimaBarra(Slider Barra, int Valor)
    {
        LeanTween.value(Barra.gameObject, Barra.value, Valor, 0.3f)//anima o valor das barras
            .setOnUpdate((float val) => {//val é o valor atual de value
                Barra.value = val;//aplica-se o valor ao value
            });
    }
}
