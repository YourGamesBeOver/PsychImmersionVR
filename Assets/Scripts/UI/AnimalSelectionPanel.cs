using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class AnimalSelectionPanel : MonoBehaviour
    {

        public Button SubmitButton;

        private AnimalType _curSelection = AnimalType.None;

        public bool BeeSelected
        {
            get { return (_curSelection & AnimalType.Bee) == AnimalType.Bee; }
            set
            {
                if (value)
                {
                    _curSelection |= AnimalType.Bee;
                }
                else
                {
                    _curSelection &= ~AnimalType.Bee;
                }
                UpdateConfirmButton();
            }
        }
        public bool MouseSelected {
            get { return (_curSelection & AnimalType.Mouse) == AnimalType.Mouse; }
            set {
                if (value) {
                    _curSelection |= AnimalType.Mouse;
                } else {
                    _curSelection &= ~AnimalType.Mouse;
                }
                UpdateConfirmButton();
            }
        }
        public bool SpiderSelected {
            get { return (_curSelection & AnimalType.Spider) == AnimalType.Spider; }
            set {
                if (value) {
                    _curSelection |= AnimalType.Spider;
                } else {
                    _curSelection &= ~AnimalType.Spider;
                }
                UpdateConfirmButton();
            }
        }

        private void UpdateConfirmButton()
        {
            SubmitButton.interactable = _curSelection != AnimalType.None;
        }

        public void Submit()
        {
            ExperimentManager.Instance.SetAnimal(_curSelection);
        }
        
    }
}
