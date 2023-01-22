using Assets.Scripts.Core;
using Assets.Scripts.Uteis.Prefabs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VectorForestScenery;

namespace Assets.Scripts.Controllers
{
    public enum InteractionType
    {
        Wind,
        Light,
        Snow,
        Addition,
        Clear
    }

    public class GameManagerController : UniqueMonoBehaviour
    {
        [SerializeField] private float _spawnRate = 3f;
        [SerializeField] private float _playTime;
        [SerializeField] private Vector2 _spawnMinMaxDistance = new Vector2(10, 50);
        [SerializeField] private int _maxEnemysSpawnAtOnce = 20;
        [SerializeField] private List<GameObject> _enemys;

        public float PlayTime { get => _playTime; set => _playTime = value; }
        public float SpawnRate { get => _spawnRate; set => _spawnRate = value; }
        public Vector2 SpawnMinMaxDistance { get => _spawnMinMaxDistance; set => _spawnMinMaxDistance = value; }
        public int MaxEnemysSpawnAtOnce { get => _maxEnemysSpawnAtOnce; set => _maxEnemysSpawnAtOnce = value; }
        public List<GameObject> Enemys { get => _enemys; set => _enemys = value; }

        public static GameManagerController Instance;
        public bool keepAttacking;
        public bool IkeepAttacking;



        #region "Inspector"
        public InteractionType _interaction;
        public Scenery _scenery;
        [Header("Wind")]
        public VectorForestScenery.Utils.Wind _wind;
        public float _windVelocity = 2;
        [Header("Light Variation")]
        public VectorForestScenery.Utils.LightVariation _lightVariation;
        public float _maxLightDistance = 10;
        public float _maxLightVariation = 0.7f;
        [Header("Snow")]
        public VectorForestScenery.Utils.Snow _snow;
        [Header("Addition")]
        public SceneryItem _prefab;
        #endregion

        private bool _canWind;
        private bool _canSpawn;
        public bool CanWind { get => _canWind; set => _canWind = value; }
        public bool CanSpawn { get => _canSpawn; set => _canSpawn = value; }

        private void Awake()
        {
            Instance = this;
            CanWind = true;
            CanSpawn = true;
            Enemys = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Enemys"));
        }

        private void FixedUpdate()
        {
            if (CanWind) StartCoroutine(WindDelay());
            //if (keepAttacking && !IkeepAttacking) StartCoroutine(AttackDelay());
            if (CanSpawn) StartCoroutine(SpawnDelay());
        }


        private void SpawnEnemysOverTime()
        {
            // Debug.Log("Spawn Enemys Called");
            // Busca todos os players
            List<GameObject> characters = GameManagerController.getAllInScene(LayerMask.GetMask("Ally"), ID);
            // Debug.Log("Personagens encontrados: " + characters.Count);
            GameObject charactersHierarchy = GameObject.FindGameObjectWithTag("CharactersHierarchy");
            // Spawna inimigos para os players
            foreach (GameObject character in characters)
            {
                Vector3 characterPosition = character.transform.position;
                float newX = Random.Range(-SpawnMinMaxDistance.y, SpawnMinMaxDistance.y);
                float newY = Random.Range(-SpawnMinMaxDistance.y, SpawnMinMaxDistance.y);
                Vector3 spawnPoint = new Vector3(newX, newY, 0);

                // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
                GameObject instance = Instantiate(Enemys[Random.Range(0, Enemys.Count)], spawnPoint, Quaternion.identity);
                instance.transform.SetParent(charactersHierarchy.transform);
                // Debug.Log("Criado enemy");
            }
        }

        public static DamagePopup DamagePopup(Vector3 position, string info, bool isCritical)
        {
            var popupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);

            DamagePopup damagePopup = popupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(info, isCritical);
            return damagePopup;
        }

        public static void PlayHitParticle(ParticleSystem prefab, Vector3 position)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }

        public static GameObject getClosestTarget(string ID, Transform transform, float range, LayerMask layer)
        {
            // Removido pois se for 0 quero que traga todos
            // if (range <= 0) return null;
            List<GameObject> objetos = GameManagerController.getAllInRange(ID, transform, range, layer);
            if (objetos.Count == 0) objetos = GameManagerController.getAllInScene(layer, ID);
            GameObject menorDistancia = null;
            float distanciaMinima = Mathf.Infinity;
            Vector3 posicaoAtual = transform.position;
            float closest = 0f;
            foreach (GameObject objeto in objetos)
            {
                Character baseController = objeto.GetComponent<Character>();
                // Se não for filho da basecontroller ignora
                if (!baseController) continue;
                // Se o personagem for igual ao próprio que está procurando ignora
                if (ID == baseController.ID) continue;
                GameObject gameObject = objeto.gameObject;
                // Calcula a distancia
                closest = Vector3.Distance(gameObject.transform.position, posicaoAtual);
                // Se a distance atual for menor que a distancia minima registrada no loop troca o target
                if (closest < distanciaMinima)
                {
                    menorDistancia = gameObject;
                    distanciaMinima = closest;
                }
            }

            // retorna o target
            return menorDistancia;
        }

        public static List<GameObject> getAllInRange(string ID, Transform transform, float range, LayerMask layer)
        {
            // Busca todos os inimigos que estão no range e na layer solicitada
            Collider2D[] nearGameobjects = Physics2D.OverlapCircleAll(transform.position, range, layer);
            List<GameObject> objects = new List<GameObject>();
            foreach (Collider2D collider in nearGameobjects)
            {
                Character baseController = collider.gameObject.GetComponent<Character>();
                // Se o personagem for igual ao próprio que está procurando ignora
                if (ID == baseController.ID) continue;
                objects.Add(collider.gameObject);
            }

            return objects;
        }

        public static List<GameObject> getAllInScene(LayerMask layer, string ID = null)
        {
            Character[] all = GameObject.FindObjectsOfType<Character>();
            List<GameObject> objetos = new List<GameObject>();
            foreach (Character character in all)
            {
                GameObject gameObject = character.gameObject;
                // Se o personagem for igual ao próprio que está procurando ignora
                if (ID != null && ID == character.ID) continue;

                // Verifica se a layer do personagem procurado é igual a layer de inimigos
                if (((layer.value >> character.gameObject.layer) & 1) == 1) objetos.Add(gameObject);
            }
            return objetos;
        }

        public static bool IsNullOrDestroyed(System.Object obj)
        {

            if (object.ReferenceEquals(obj, null)) return true;

            if (obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;

            return false;
        }

        private void Interaction(bool operation)
        {
            Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Wind(player.transform.position + new Vector3(30, player.transform.position.y, player.transform.position.z), operation);
            // switch (_interaction)
            // {
            //     case InteractionType.Wind:

            //         break;

            //     case InteractionType.Light:
            //         LightVariation(origin, operation);
            //         break;

            //     case InteractionType.Snow:
            //         Snow(origin, operation);
            //         break;

            //     case InteractionType.Addition:
            //         Addition(origin, operation);
            //         break;
            // }
        }

        private void Wind(Vector2 origin, bool operation)
        {
            _wind.StartWind(origin, _windVelocity, 100);
        }

        private void Snow(Vector2 origin, bool operation)
        {
            if (operation)
            {
                _snow.SetSnow();
            }
            else
            {
                _snow.RemoveSnow();
            }
        }

        private void LightVariation(Vector2 origin, bool operation)
        {
            if (operation)
            {
                _lightVariation.SetVariation(_maxLightVariation, origin, _maxLightDistance);
            }
            else
            {
                _lightVariation.RemoveVariation();
            }

        }

        private void Addition(Vector2 origin, bool operation)
        {
            _scenery.Add(origin, _prefab);
        }

        private bool IsOverInterface()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }


        //public IEnumerator AttackDelay()
        //{
        //    IkeepAttacking = true;
        //    yield return new WaitForSeconds(1);
        //    TesteAnimator[] animators = GameObject.FindObjectsOfType<TesteAnimator>();
        //    foreach (TesteAnimator animator in animators)
        //    {
        //        Animator anim = animator.GetComponent<Animator>();
        //        anim.SetTrigger("attack");
        //    }
        //    IkeepAttacking = false;
        //}

        public IEnumerator WindDelay()
        {
            CanWind = false;
            yield return new WaitForSeconds(Random.Range(0, 100));
            Interaction(true);
            CanWind = true;
        }
        public IEnumerator SpawnDelay()
        {
            CanSpawn = false;
            yield return new WaitForSeconds(SpawnRate);
            SpawnEnemysOverTime();
            CanSpawn = true;
        }

    }



}


