using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class UnitControl : NetworkBehaviour
{
    [Header("Base Setup")]
    public float walkSpeed = 7.5f;
    public float runSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravitys = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController charCtrl;
    Vector3 moveDirection = Vector3.zero;
    float rotateX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float camYOffset = 0.4f;
    private Camera playerCam;

    [Header("Animator Setip")]
    public Animator anim;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCam = Camera.main;
            playerCam.transform.position = new Vector3(transform.position.x, transform.position.y + camYOffset, transform.position.z);
            playerCam.transform.SetParent(transform);
        }
        else
        {
            gameObject.GetComponent<UnitControl>().enabled = false;
        }
    }

    void Start()
    {
        charCtrl = GetComponent<CharacterController>();

        //커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        bool isWalking = false;

        //왼쪽 쉬프트 누르면 걷게
        isWalking = Input.GetKey(KeyCode.LeftShift);

        //땅에 붙어있는지 체크하고, 그렇다면 axis 기준으로 움직임 계산
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isWalking ? walkSpeed : runSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isWalking ? walkSpeed : runSpeed) * Input.GetAxis("Horizontal") : 0;
        float moveDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && charCtrl.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = moveDirectionY;
        }

        if (!charCtrl.isGrounded)
        {
            moveDirection.y -= gravitys * Time.deltaTime;
        }

        //움직여잇!
        charCtrl.Move(moveDirection * Time.deltaTime);

        //캐릭터랑 카메라 돌리기
        if (canMove && playerCam != null)
        {
            rotateX += Input.GetAxis("Mouse Y") * lookSpeed;
            rotateX = Mathf.Clamp(rotateX, -lookXLimit, lookXLimit);
            playerCam.transform.localRotation = Quaternion.Euler(rotateX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
