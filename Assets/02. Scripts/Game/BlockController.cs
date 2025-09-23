using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private Block[] blocks;
    [SerializeField] private GameObject blockPrefab;

    public delegate void OnBlockClicked(int row, int col);
    public OnBlockClicked OnBlockClickedDelegate;

    // 1. 모든 Block을 초기화
    public void InitBlocks()
    {
        float colStartPos = -12.6f;
        float rowStartPos = 12.6f;

        blocks = new Block[15 * 15];

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                var blockObject = Instantiate(blockPrefab, transform);
                blockObject.transform.localPosition =
                    new Vector3(colStartPos + (1.8f * j), rowStartPos - (1.8f * i), 0);

                var blockIndex = i * 15 + j;

                Block block = blockObject.GetComponent<Block>();
                block.InitMarker(blockIndex, blockIndex =>
                {
                    // 특정 Block이 클릭 된 상태에 대한 처리
                    var row = blockIndex / Constants.BlockColumnCount;
                    var col = blockIndex % Constants.BlockColumnCount;
                    OnBlockClickedDelegate?.Invoke(row, col);
                });
                blocks[i * 15 + j] = block;
            }
        }
    }

    // 2. 특정 Block에 마커 표시
    public void PlaceMaker(Block.MarkerType markerType, int row, int col)
    {
        // row, col >> index 변환
        var blockIndex = row * Constants.BlockColumnCount + col;
        blocks[blockIndex].SetMarker(markerType);
    }

    // 3. 특정 Block의 배경색을 설정
    public void SetBlockColor()
    {
        // TODO: 게임 로직이 완성되면 구현
    }
}