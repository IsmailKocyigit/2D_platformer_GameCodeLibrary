using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Get mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 shootDirection = (mousePosition - transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;

            Destroy(bullet, 2f);
        }
    }
}
