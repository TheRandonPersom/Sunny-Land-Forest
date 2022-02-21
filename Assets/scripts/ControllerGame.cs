using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGame : MonoBehaviour
{
    private int score;
    public Text txtScore;
    public GameObject hitprefab;

    public Sprite[] imgensVida;
    public Image barraVida;

    public AudioSource fxGame;
    public AudioClip fxCenouraColetada;
    public AudioClip fxExplosao;
    public AudioClip fxDie;

    public void Pontuacao(int qtdPontos)
    {
        score += qtdPontos;
        txtScore.text = score.ToString();

        //som da coleta da cenoura
        fxGame.PlayOneShot(fxCenouraColetada);

    }

    public void BarraVida( int healthVida )
    {
        barraVida.sprite = imgensVida[healthVida];
    }

}
