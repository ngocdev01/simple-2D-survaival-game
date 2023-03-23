using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatMenu : MonoBehaviour
{
    public PlayerController playerController ;

    public TextMeshProUGUI ATK;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI ATKSPEED;
    public TextMeshProUGUI SPEED;

    public void LoadStat(PlayerController playerController)
    {
        ATK.text = $"ATK {playerController.ATK.Value}";
        HP.text = $"HP {playerController.HP.Value} / {playerController.HP.statBaseValue.Value}";
        ATKSPEED.text = $"ATK SPEED {playerController.ATKSpeed.Value}";
        SPEED.text = $"SPEED {playerController.Speed.Value}";
     
    }

}
