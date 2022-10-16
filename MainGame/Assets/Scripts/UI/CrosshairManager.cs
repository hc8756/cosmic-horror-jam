using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class CrosshairManager : MonoBehaviour
    {
        public Image CrosshairImage;
        public Sprite NullCrosshairSprite;
        public float CrosshairUpdateshrpness = 5f;

        private PlayerWeaponsManager _playerWeaponsManager;
        private InteractablesManager _interactablesManager;
        private bool _wasPointingAtEnemy;
        private RectTransform _crosshairRectTransform;
        private CrosshairData _crosshairDataDefault;
        private CrosshairData _crosshairDataTarget;
        private CrosshairData _currentCrosshair;

        void Start()
        {
            _playerWeaponsManager = UnityHelper.FindObjectOfTypeOrThrow<PlayerWeaponsManager>();
            _interactablesManager = UnityHelper.FindObjectOfTypeOrThrow<InteractablesManager>();

            OnWeaponChanged(_playerWeaponsManager.GetActiveWeapon());
            _playerWeaponsManager.OnSwitchedToWeapon += OnWeaponChanged;
            _interactablesManager.OnHoveredInteractableChanged += OnInteractableHover;
        }
        
        void Update()
        {
            UpdateCrosshairPointingAtEnemy(false);
            _wasPointingAtEnemy = _playerWeaponsManager.IsPointingAtEnemy;
        }

        void UpdateCrosshairPointingAtEnemy(bool force)
        {
            if (_crosshairDataDefault.CrosshairSprite == null)
                return;

            if ((force || !_wasPointingAtEnemy) && _playerWeaponsManager.IsPointingAtEnemy)
            {
                _currentCrosshair = _crosshairDataTarget;
                CrosshairImage.sprite = _currentCrosshair.CrosshairSprite;
                _crosshairRectTransform.sizeDelta = _currentCrosshair.CrosshairSize * Vector2.one;
            }
            else if ((force || _wasPointingAtEnemy) && !_playerWeaponsManager.IsPointingAtEnemy)
            {
                _currentCrosshair = _crosshairDataDefault;
                CrosshairImage.sprite = _currentCrosshair.CrosshairSprite;
                _crosshairRectTransform.sizeDelta = _currentCrosshair.CrosshairSize * Vector2.one;
            }

            CrosshairImage.color = Color.Lerp(CrosshairImage.color, _currentCrosshair.CrosshairColor,
                Time.deltaTime * CrosshairUpdateshrpness);

            _crosshairRectTransform.sizeDelta = Mathf.Lerp(_crosshairRectTransform.sizeDelta.x,
                _currentCrosshair.CrosshairSize,
                Time.deltaTime * CrosshairUpdateshrpness) * Vector2.one;
        }

        void OnWeaponChanged(WeaponController newWeapon)
        {
            if (newWeapon)
            {
                CrosshairImage.enabled = true;
                _crosshairDataDefault = newWeapon.CrosshairDataDefault;
                _crosshairDataTarget = newWeapon.CrosshairDataTargetInSight;
                _crosshairRectTransform = CrosshairImage.GetComponent<RectTransform>();
                DebugUtility.HandleErrorIfNullGetComponent<RectTransform, CrosshairManager>(_crosshairRectTransform,
                    this, CrosshairImage.gameObject);
            }
            else
            {
                if (NullCrosshairSprite)
                {
                    CrosshairImage.sprite = NullCrosshairSprite;
                }
                else
                {
                    CrosshairImage.enabled = false;
                }
            }

            UpdateCrosshairPointingAtEnemy(true);
        }

        private void OnInteractableHover(Interactable interactable)
        {
            if (interactable)
            {
                CrosshairImage.enabled = true;
                _crosshairDataDefault = interactable.crosshairData;
                _crosshairDataTarget = interactable.crosshairDataTarget;
                _crosshairRectTransform = CrosshairImage.GetComponent<RectTransform>();
                DebugUtility.HandleErrorIfNullGetComponent<RectTransform, CrosshairManager>(_crosshairRectTransform,
                    this, CrosshairImage.gameObject);
            }
            else
            {
                if (NullCrosshairSprite)
                {
                    CrosshairImage.sprite = NullCrosshairSprite;
                }
                else
                {
                    CrosshairImage.enabled = false;
                }
            }

            UpdateCrosshairPointingAtEnemy(false);
        }
    }
}