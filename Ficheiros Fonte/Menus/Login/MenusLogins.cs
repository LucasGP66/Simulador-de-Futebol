using System.Collections.Generic;
using UnityEngine;

public class MenusLogins : MonoBehaviour//tal como o TodosOsMenus, mas para a scene do login
{
    public List<GameObject> menus;

    public static MenusLogins Instance;

    private void Awake()
    {
        Instance = this;
    }


    //tive um erro estranho aqui, tentei usar um foreach para iterar pelos menus, mas dava null reference
    //não percebo muito bem as coisas, mas pelos vistos SetActive modifica de certo modo a lista
    //e o foreach não aguenta que a lista esteja a ser mexida enquanto está a iterar
    //utilizar um for loop resolve isso
    public void MudaMenu(GameObject menuAbrir)
    {

        for (int i = menus.Count - 1; i >= 0; i--)
        {
            var menu = menus[i];
            if (!menu) continue;

            menu.SetActive(menu == menuAbrir);
        }
    }
}
