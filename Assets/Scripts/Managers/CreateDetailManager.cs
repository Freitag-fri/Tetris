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
            prefabDetailsCount = prefabDetails.Length;

            var gameManager = GameManager.Instance;
            if (gameManager == null || gameManager.BlockSkin == null || gameManager.BoardSkin == null)
                return;

            blockMaterial.mainTexture = gameManager.BlockSkin.Material.mainTexture;
            boardMaterial.mainTexture = gameManager.BoardSkin.Material.mainTexture;
            boardMaterial.SetTextureScale("_BaseMap", gameManager.BoardSkin.Material.GetTextureScale("_BaseMap"));
            ghostMaterial.mainTexture = gameManager.BlockSkin.Material.mainTexture;
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