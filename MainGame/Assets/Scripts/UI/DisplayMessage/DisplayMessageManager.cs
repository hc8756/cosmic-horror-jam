using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class DisplayMessageManager : EventHandler<string>
    {
        struct Message
        {
            public string message;
            public NotificationToast notification;
        }
        public UITable DisplayMessageRect;
        public NotificationToast MessagePrefab;

        List<Message> m_PendingMessages;

        void Awake()
        {
            m_PendingMessages = new List<Message>();
        }

        public override void OnEventRaised(string message)
        {
            NotificationToast notification = Instantiate(MessagePrefab, DisplayMessageRect.transform).GetComponent<NotificationToast>();

            m_PendingMessages.Add(new Message()
            {
                message = message,
                notification = notification,
            });
        }
        
        void Update()
        {
            foreach (var message in m_PendingMessages)
            {
                message.notification.Initialize(message.message);
                DisplayMessage(message.notification);
            }

            // Clear deprecated messages
            m_PendingMessages.RemoveAll(x => x.notification.Initialized);
        }

        void DisplayMessage(NotificationToast notification)
        {
            DisplayMessageRect.UpdateTable(notification.gameObject);
            //StartCoroutine(MessagePrefab.ReturnWithDelay(notification.gameObject, notification.TotalRunTime));
        }
    }
}