using UnityEngine;

public class GridMover : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float resetPositionX; // このX位置に到達したらリセット
    public float startPositionX; // リセット後のスタート位置

    private Transform[] grids = new Transform[2];
    private float gridWidth;

    void Start()
    {
        grids[0] = transform;
        grids[1] = Instantiate(transform, transform.parent);

        gridWidth = grids[0].GetComponent<SpriteRenderer>().bounds.size.x;

        // 2つ目の背景の位置を初期設定
        grids[1].position = new Vector3(grids[0].position.x + gridWidth, grids[0].position.y, grids[0].position.z);
    }

    void Update()
    {
        foreach (Transform grid in grids)
        {
            // 左方向に移動
            grid.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            // 指定した位置に到達したら位置をリセット
            if (grid.position.x <= resetPositionX)
            {
                Vector3 resetPosition = new Vector3(grid.position.x + 2 * gridWidth, grid.position.y, grid.position.z);
                grid.position = resetPosition;
            }
        }
    }
}
