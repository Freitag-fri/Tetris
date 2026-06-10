using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class DebugModule: MonoBehaviour
	{
        public BoardController boardController;

        public Material materialEmptyPossition;
        public Material materialFullPossition;
        public GameObject prefabForDebug;
        public GameObject[] boardDebugingDetails = new GameObject[200]; // get

        // Use this for initialization
        void Start()
		{
            GameObject pointPossition = new GameObject("pointPossition"); /*Instantiate(prefabDetails[2], new Vector2(15, 0));*/
            pointPossition.transform.position = new Vector2(15, 0);
            int boardPositionsLength = boardDebugingDetails.Length;
            for (int i = 0; i < boardPositionsLength; i++)
            {
                int row = i / 10;
                int col = i % 10;
                GameObject newObject = Instantiate(prefabForDebug, pointPossition.transform/*, new Vector2(row, col)*/);
                newObject.transform.localPosition = new Vector2(col, -row);
                boardDebugingDetails[i] = newObject;
            }

            boardController.CreateDetailEvent += ReDrawDebugingObjects;
        }

        void ReDrawDebugingObjects()
        {
            var boardPositions = boardController.boardPositions;
            var boardPositionsLength = boardPositions.Length;
            for (int i = 0; i < boardPositionsLength; i++)
            {
                if (boardPositions[i] != null)
                {
                    boardDebugingDetails[i].GetComponent<Renderer>().material = materialFullPossition;
                }
                else
                {
                    boardDebugingDetails[i].GetComponent<Renderer>().material = materialEmptyPossition;
                }
            }
        }
    }
}