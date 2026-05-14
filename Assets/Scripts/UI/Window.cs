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

            if (windowAnimator != null)
                windowAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        public virtual void Initialize() { }

        public void Show(bool isImmediately)
        {
            transform.SetAsLastSibling();
            OpenStart();

            Animator animator = WindowAnimator;

            if (animator != null)
                animator.Play(isImmediately ? idleAnimationName : openAnimationName);

            if (isImmediately || animator == null)
                OpenEnd();
        }

        public virtual void Hide(bool isImmediately)
        {
            CloseStart();

            Animator animator = WindowAnimator;

            if (gameObject.activeInHierarchy && animator != null)
                animator.Play(isImmediately ? hiddenAnimationName : closeAnimationName);

            if (isImmediately || animator == null)
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
