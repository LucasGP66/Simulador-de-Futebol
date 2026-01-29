using TMPro;
using UnityEngine;

//gere o menu de carreira
public class CarreiraMenuManager : MonoBehaviour
{
    public TMP_Text txtClube;
    public TMP_Text txtContratoNum;
    public GameObject menuNegociarFechado;


    public void CarreiraAtualiza()
    {
        menuNegociarFechado.SetActive(true);
        AtualizaCLube();
        AtualizaContrato();
    }

    private void AtualizaCLube()
    {
        var player = PlayerManager.Instance.Player;
        if (player.Contrato <= 0) txtClube.text = "Agente Livre";
        else txtClube.text = PlayerManager.Instance.Player.Clube;
    }

    private void AtualizaContrato()
    {
        var player = PlayerManager.Instance.Player;
        if (player.Contrato <= 0) txtContratoNum.text = "0";
        else txtContratoNum.text = PlayerManager.Instance.Player.Contrato.ToString();

    }

}
