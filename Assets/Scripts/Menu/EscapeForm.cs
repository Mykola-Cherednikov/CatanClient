using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class EscapeForm : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(gameObject);
            }
        }

        public void Disconnect()
        {
            Multiplayer.Instance.Disconnect();
            Destroy(gameObject);
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}
