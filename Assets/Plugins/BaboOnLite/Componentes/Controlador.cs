using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BaboOnLite
{
    [DefaultExecutionOrder(-1)]
    [AddComponentMenu("BaboOnLite/Controlador")]
    [DisallowMultipleComponent]
    //[HelpURL("")]

    public partial class Controlador {
        //Instancias estaticas--------------------------------------
        //Tiempo
        public delegate Coroutine C1(float t, Action f, bool bucle = false);
        //Movimiento
        public delegate Coroutine C2(Transform t, Movimiento m);
        public delegate Coroutine C3(RectTransform t, Movimiento m);
        //Rotacion
        public delegate Coroutine C4(Transform t, Rotacion r);
        public delegate Coroutine C5(RectTransform t, Rotacion r);
        //Rotacion
        public delegate Coroutine C6(SpriteRenderer t, Color c, float duracion);
        public delegate Coroutine C7(Image t, Color c, float duracion);
        public delegate Coroutine C8(Camera t, Color c, float duracion);
        public delegate Coroutine C9(Renderer t, Color c, float duracion);
        public delegate Coroutine C10(TextMeshProUGUI t, Color c, float duracion);

        //Rotacion
        public delegate void C11(string nombre, float duracion);
        public delegate bool C12(string nombre);

        public static C1 Rutina;
        public static C2 Mover;
        public static C3 MoverCanva;
        public static C4 Rotar;
        public static C5 RotarCanva;
        public static C6 ColorSpriteRender;
        public static C7 ColorImage;
        public static C8 ColorCamara;
        public static C9 ColorRender;
        public static C10 ColorText;
        public static C11 IniciarEspera;
        public static C12 Esperando;

        //Variables privadas------------------------------------------
        private Trans moviendo = new Trans();
        private Trans rotando = new Trans();

        private Dictionary<string, bool> espera = new Dictionary<string, bool>();
    }
    public partial class Controlador : MonoBehaviour
    {
        private void Start(){
            Rutina = rutina;
            Mover = mover;
            MoverCanva = mover;
            Rotar = rotar;
            RotarCanva = rotar;
            ColorSpriteRender = color;
            ColorImage = color;
            ColorCamara = color;
            ColorRender = color;
            ColorText = color;
            IniciarEspera = iniciarEspera;
            Esperando = esperando;
        }

        //Rutina-----------------------------------------------------
        #region tiempo
        public Coroutine rutina(float t, Action f, bool bucle = false) { //LLAMADA
            return StartCoroutine(Corrutina(
                t, f, bucle
            ));
        }

        private IEnumerator Corrutina(float t, Action f, bool bucle) { //EJECUCION
            yield return new WaitForSeconds(t);
            f.Invoke();

            if (bucle) StartCoroutine(Corrutina(
                t, f, bucle
            ));
        }
        #endregion

        //Movimiento-------------------------------------------------
        #region movimiento
        private Coroutine mover<T>(T t, Movimiento m) { //LLAMADA
            if (t is Transform trans) {
                if (moviendo.trans.Some((e) => e == trans))
                {
                    return null;
                }
                moviendo.trans.Add(trans);
            }
            if (t is RectTransform rect)
            {
                if (moviendo.rect.Some((e) => e == rect))
                {
                    return null;
                }
                moviendo.rect.Add(rect);
            }

            return StartCoroutine(MoverCorrutina(
                t, m
            ));
        }
  
        private IEnumerator MoverCorrutina<T>(T t, Movimiento m) { //EJECUCION

            Vector3 posicionInicial = Vector3.zero;
            float tiempoPasado = 0f;

            if (t is Transform trans1) posicionInicial = trans1.localPosition;
            if (t is RectTransform rect1) posicionInicial = rect1.anchoredPosition;

            while (tiempoPasado < m.duracion)
            {
                float tiempo = tiempoPasado / m.duracion;
                float velocidadActual = m.curva.Evaluate(tiempo);

                Vector3 nuevaPosicion = Vector3.Lerp(posicionInicial, m.destino, velocidadActual);
                if (t is Transform trans2) trans2.localPosition = nuevaPosicion;
                if (t is RectTransform rect2) rect2.localPosition = nuevaPosicion;

                tiempoPasado += Time.deltaTime;
                yield return null;
            }

            if (t is Transform trans3) trans3.localPosition = m.destino;
            if (t is RectTransform rect3) rect3.localPosition = m.destino;

            if (t is Transform trans4) moviendo.trans.Remove(trans4);
            if (t is RectTransform rect4) moviendo.rect.Remove(rect4);
        }
        #endregion

        //Rotacion---------------------------------------------------
        #region rotacion
        private Coroutine rotar<T>(T t, Rotacion r) { //LLAMADA
            if (t is Transform trans)
            {
                if (rotando.trans.Some((e) => e == trans))
                {
                    return null;
                }
                rotando.trans.Add(trans);
            }
            if (t is RectTransform rect)
            {
                if (rotando.rect.Some((e) => e == rect))
                {
                    return null;
                }
                rotando.rect.Add(rect);
            }

            return StartCoroutine(RotarCorrutina(
                t, r
            ));
        }

        private IEnumerator RotarCorrutina<T>(T t, Rotacion r) { //EJECUCION

            Quaternion rotacionInicial = Quaternion.Euler(0, 0, 0);
            float tiempoPasado = 0f;

            if (t is Transform trans1) rotacionInicial = trans1.rotation;
            if (t is RectTransform rect1) rotacionInicial = rect1.rotation;

            while (tiempoPasado < r.duracion)
            {
                float tiempo = tiempoPasado / r.duracion;
                float velocidadActual = r.curva.Evaluate(tiempo);

                Quaternion nuevaRotacion = Quaternion.Lerp(rotacionInicial, Quaternion.Euler(0 ,0, r.destino), velocidadActual);
                if (t is Transform trans2) trans2.rotation = nuevaRotacion;
                if (t is RectTransform rect2) rect2.rotation = nuevaRotacion;

                tiempoPasado += Time.deltaTime;
                yield return null;
            }

            if (t is Transform trans3) trans3.rotation = Quaternion.Euler(0, 0, r.destino);
            if (t is RectTransform rect3) rect3.rotation = Quaternion.Euler(0, 0, r.destino);

            if (t is Transform trans4) rotando.trans.Remove(trans4);
            if (t is RectTransform rect4) rotando.rect.Remove(rect4);
        }
        #endregion

        //Color------------------------------------------------------
        #region color
        private Coroutine color<T>(T f, Color c, float duracion) { //LLAMADA
            return StartCoroutine(ColorCorrutina(
                f, c, duracion
            ));
        }

        private IEnumerator ColorCorrutina<T>(T f, Color c2, float duracion)
        { //EJECUCION

            Color c1 = Color.white;
            float tiempoTotal = 0f;

            if (f is Camera cam1) c1 = cam1.backgroundColor;
            if (f is SpriteRenderer sprite1) c1 = sprite1.color;
            if (f is Image color1) c1 = color1.color;
            if (f is Renderer render1) c1 = render1.material.color;
            if (f is TextMeshProUGUI text1) c1 = text1.color;

            while (tiempoTotal < duracion)
            {
                float t = tiempoTotal / duracion;

                Color c = DarColor(c1, c2, t);

                if (f is Camera cam2) cam2.backgroundColor = c;
                if (f is SpriteRenderer sprite2) sprite2.color = c;
                if (f is Image color2) color2.color = c;
                if (f is Renderer render2) render2.material.color = c;
                if (f is TextMeshProUGUI text2) text2.color = c;

                yield return null;
                tiempoTotal += Time.deltaTime;
            }

            if (f is Camera finalCam) finalCam.backgroundColor = c2;
            if (f is SpriteRenderer finalSprite) finalSprite.color = c2;
            if (f is Image finalImage) finalImage.color = c2;
            if (f is Renderer finalRender) finalRender.material.color = c2;
            if (f is TextMeshProUGUI finalText) finalText.color = c2;
        }

        private Color DarColor(Color c1, Color c2, float duracion) { 
            return Color.Lerp(
                    c1,
                    c2,
                    duracion
            );
        }

        #endregion

        //Espera-----------------------------------------------------
        #region espera
        private void iniciarEspera(string nombre, float duracion) {
            espera[nombre] = false;
            Rutina(duracion, () => {
                espera[nombre] = true;
            });
        }
        private bool esperando(string nombre) {
            if (!espera.ContainsKey(nombre)) {
                espera[nombre] = true;
            }
            return espera[nombre];
        }
        #endregion

    }

    //Instancia--------------------------------------------------
    #region
    public class Instanciar<T> : MonoBehaviour
    {
        private static Dictionary<string, T> instancias = new Dictionary<string, T>();

        public static void A�adir(string nombre, T instancia, GameObject objeto = null, bool global = false)
        {

            if (instancias.Some((i) => EqualityComparer<T>.Default.Equals(instancia, i.Value)))
            {
                Debug.LogWarning("Ya tienes un objeto igual guardado");
            }


            if (instancias.Every((element) => nombre != element.Key))
            {
                if (global)
                {
                    DontDestroyOnLoad(objeto);
                }
                instancias.Add(nombre, instancia);
            }
            else
            {
                Debug.LogWarning("Ya tienes un objeto con ese nombre");
                Destroy(objeto);
            }
        }

        public static T Coger(string nombre)
        {
            return instancias[nombre];
        }
    }
    #endregion
}
