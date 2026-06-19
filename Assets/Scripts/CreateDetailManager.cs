using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
	public class CreateDetailManager : MonoBehaviour
	{

        [SerializeField] private GameObject[] prefabDetails; // todo check is empty in start()
        [SerializeField] private GameObject nextDetailPosition;
        
		private GameObject nextDetail;
        private int prefabDetailsCount;

        [SerializeField] private Material blockMaterioal;
        [SerializeField] private Material boardMaterioal;

        private void Awake()
        {
            blockMaterioal.mainTexture = GameManager.Instance.BlockMaterial.mainTexture;
            boardMaterioal.mainTexture = GameManager.Instance.BoardMaterial.mainTexture;
        }

        // Use this for initialization
        void Start()
		{
            prefabDetailsCount = prefabDetails.Length;
        }

        private void createNextDetail() 
        {
            int numberRandomDetail = UnityEngine.Random.Range(0, prefabDetailsCount);
            nextDetail = Instantiate(prefabDetails[numberRandomDetail], nextDetailPosition.transform);

            var l_object = nextDetail.GetComponent<Detail>();
            l_object.Initialize();
        }

        public GameObject returnNextDetailAndCreateNew()
        {
            if(nextDetail == null)
            {
                createNextDetail();
            }
            var newActualDdetail = nextDetail;
            createNextDetail();
            return newActualDdetail;
        }
	}
}