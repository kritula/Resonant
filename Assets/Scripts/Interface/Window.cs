using UnityEngine;

namespace OmniumLessons
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] private string windowName;

        [Space(10)][SerializeField] private Animator windowAnimator;
        [SerializeField] protected string openAnimationName = "Open";
        [SerializeField] protected string idleAnimationName = "Idle";
        [SerializeField] protected string closeAnimationName = "Close";
        [SerializeField] protected string hiddenAnimationName = "Hidden";

        public bool IsOpened { get; protected set; } = false;

        protected Animator WindowAnimator
        {
            get
            {
                if (windowAnimator == null)
                    windowAnimator = GetComponent<Animator>();

                return windowAnimator;
            }
        }

        private void Awake()
        {
            if (windowAnimator == null)
                windowAnimator = GetComponent<Animator>();

            windowAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        public virtual void Initialize() { }

        public void Show(bool isImmediately)
        {
            OpenStart();
            WindowAnimator.Play(isImmediately ? idleAnimationName : openAnimationName);

            if (isImmediately)
                OpenEnd();
        }

        public void Hide(bool isImmediately)
        {
            CloseStart();
            if (gameObject.activeInHierarchy)
                WindowAnimator.Play(isImmediately ? hiddenAnimationName : closeAnimationName);

            if (isImmediately)
                CloseEnd();
        }

        protected virtual void OpenStart()
        {
            this.gameObject.SetActive(true);
            IsOpened = true;
        }

        protected virtual void OpenEnd()
        {

        }

        protected virtual void CloseStart()
        {
            IsOpened = false;
        }

        protected virtual void CloseEnd()
        {
            this.gameObject.SetActive(false);
        }
    }
}