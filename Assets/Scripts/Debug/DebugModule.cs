using UnityEngine;

namespace Assets.Scripts
{
	public class DebugModule: MonoBehaviour
	{
        public BoardController boardController;

        public Material materialEmptyPosition;
        public Material materialFullPosition;
        public GameObject prefabForDebug;
        public GameObject[] boardDebuggingDetails = new GameObject[200]; // get

        // Use this for initialization
        void Start()
		{
            GameObject pointPosition = new GameObject("pointPosition"); /*Instantiate(prefabDetails[2], new Vector2(15, 0));*/
            pointPosition.transform.position = new Vector2(15, 0);
            int boardPositionsLength = boardDebuggingDetails.Length;
            for (int i = 0; i < boardPositionsLength; i++)
            {
                int row = i / 10;
                int col = i % 10;
                GameObject newObject = Instantiate(prefabForDebug, pointPosition.transform/*, new Vector2(row, col)*/);
                newObject.transform.localPosition = new Vector2(col, -row);
                boardDebuggingDetails[i] = newObject;
            }

            boardController.CreateDetailEvent += RedrawDebuggingObjects;
        }

        void RedrawDebuggingObjects()
        {
            var boardPositions = boardController.boardPositions;
            var boardPositionsLength = boardPositions.Length;
            for (int i = 0; i < boardPositionsLength; i++)
            {
                if (boardPositions[i] != null)
                {
                    boardDebuggingDetails[i].GetComponent<Renderer>().material = materialFullPosition;
                }
                else
                {
                    boardDebuggingDetails[i].GetComponent<Renderer>().material = materialEmptyPosition;
                }
            }
        }
    }
}