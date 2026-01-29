using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.Rendering;
using UEngine = UnityEngine;

//Contem todas as classes


#region save

[FirestoreData]
[System.Serializable]
public class Save //guarda 3 jogadores
{
    [FirestoreProperty] public Jogador Save1 { get; set; }
    [FirestoreProperty] public Jogador Save2 { get; set; }
    [FirestoreProperty] public Jogador Save3 { get; set; }

    public Save()
    { }

}
#endregion

#region dados scene

[System.Serializable]
public static class DadosScene// classe estatica que guarda informações importantes
{
    public static bool IsLogin = false;
    public static bool IsReforma = false;
    public static bool IsAutenticado = false;
    public static List<int> SavesValidos = new List<int> ();
    public static int SaveNum;
}

#endregion


