using UnityEngine;

public class GridMover : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float resetPositionX; // ����X�ʒu�ɓ��B�����烊�Z�b�g
    public float startPositionX; // ���Z�b�g��̃X�^�[�g�ʒu

    private Transform[] grids = new Transform[2];
    private float gridWidth;

    void Start()
    {
        grids[0] = transform;
        grids[1] = Instantiate(transform, transform.parent);

        gridWidth = grids[0].GetComponent<SpriteRenderer>().bounds.size.x;

        // 2�ڂ̔w�i�̈ʒu�������ݒ�
        grids[1].position = new Vector3(grids[0].position.x + gridWidth, grids[0].position.y, grids[0].position.z);
    }

    void Update()
    {
        foreach (Transform grid in grids)
        {
            // �������Ɉړ�
            grid.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            // �w�肵���ʒu�ɓ��B������ʒu�����Z�b�g
            if (grid.position.x <= resetPositionX)
            {
                Vector3 resetPosition = new Vector3(grid.position.x + 2 * gridWidth, grid.position.y, grid.position.z);
                grid.position = resetPosition;
            }
        }
    }
}
