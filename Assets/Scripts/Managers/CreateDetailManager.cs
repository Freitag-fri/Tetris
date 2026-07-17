using UnityEngine;

namespace Assets.Scripts
{
	public class CreateDetailManager : MonoBehaviour
	{
        [SerializeField] private GameObject[] prefabDetails; // todo check is empty in start()
        [SerializeField] private GameObject nextDetailPosition;
        
		private GameObject nextDetail;
        private int prefabDetailsCount;

        [SerializeField] private Material blockMaterial;
        [SerializeField] private Material boardMaterial;
        [SerializeField] private Material ghostMaterial;

        private void Awake()
        {

            blockMaterial.mainTexture = GameManager.Instance.BlockMaterial.mainTexture;
            boardMaterial.mainTexture = GameManager.Instance.BoardMaterial.mainTexture;
            ghostMaterial.mainTexture = GameManager.Instance.BlockMaterial.mainTexture;
        }

        // Use this for initialization
        void Start()
		{
            prefabDetailsCount = prefabDetails.Length;
        }

        private void CreateNextDetail() 
        {
            int numberRandomDetail = UnityEngine.Random.Range(0, prefabDetailsCount);
            nextDetail = Instantiate(prefabDetails[numberRandomDetail], nextDetailPosition.transform);

            var l_object = nextDetail.GetComponent<Detail>();
            l_object.Initialize();
        }

        public GameObject ReturnNextDetailAndCreateNew()
        {
            if(nextDetail == null)
            {
                CreateNextDetail();
            }
            var newActualDetail = nextDetail;
            CreateNextDetail();
            return newActualDetail;
        }
	}
}