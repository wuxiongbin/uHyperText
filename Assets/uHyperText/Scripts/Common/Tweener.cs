using UnityEngine;
using UnityEngine.Events;

namespace WXB
{
    public class Tweener
    {
        public enum Method
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut,
            BounceIn,
            BounceOut,
        }

        public enum Style
        {
            Once,
            Loop,
            PingPong,
        }

        public Method method = Method.Linear;

        public Style style = Style.Once;

        public float duration = 1f;

        float mDuration = 0f;
        float mAmountPerDelta = 1000f;
        float mFactor = 0f;

        /// <summary>
        /// Amount advanced per delta time.
        /// </summary>

        public float amountPerDelta
        {
            get
            {
                if (duration == 0f) return 1000f;

                if (mDuration != duration)
                {
                    mDuration = duration;
                    mAmountPerDelta = Mathf.Abs(1f / duration) * Mathf.Sign(mAmountPerDelta);
                }
                return mAmountPerDelta;
            }
        }

        /// <summary>
        /// Tween factor, 0-1 range.
        /// </summary>

        public float tweenFactor { get { return mFactor; } set { mFactor = Mathf.Clamp01(value); } }

        public void Update(float delta)
        {
            // Advance the sampling factor
            mFactor += (duration == 0f) ? 1f : amountPerDelta * delta;

            // Loop style simply resets the play factor after it exceeds 1.
            if (style == Style.Loop)
            {
                if (mFactor > 1f)
                {
                    mFactor -= Mathf.Floor(mFactor);
                }
            }
            else if (style == Style.PingPong)
            {
                // Ping-pong style reverses the direction
                if (mFactor > 1f)
                {
                    mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                    mAmountPerDelta = -mAmountPerDelta;
                }
                else if (mFactor < 0f)
                {
                    mFactor = -mFactor;
                    mFactor -= Mathf.Floor(mFactor);
                    mAmountPerDelta = -mAmountPerDelta;
                }
            }

            // If the factor goes out of range and this is a one-time tweening operation, disable the script
            if ((style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f))
            {
                mFactor = Mathf.Clamp01(mFactor);
                Sample(mFactor, true);
            }
            else
            {
                Sample(mFactor, false);
            }
        }

        /// <summary>
        /// Sample the tween at the specified factor.
        /// </summary>

        public void Sample(float factor, bool isFinished)
        {
            // Calculate the sampling value
            float val = Mathf.Clamp01(factor);

            if (method == Method.EaseIn)
            {
                val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val));
            }
            else if (method == Method.EaseOut)
            {
                val = Mathf.Sin(0.5f * Mathf.PI * val);
            }
            else if (method == Method.EaseInOut)
            {
                const float pi2 = Mathf.PI * 2f;
                val = val - Mathf.Sin(val * pi2) / pi2;
            }
            else if (method == Method.BounceIn)
            {
                val = BounceLogic(val);
            }
            else if (method == Method.BounceOut)
            {
                val = 1f - BounceLogic(1f - val);
            }

            if (OnUpdate != null)
                OnUpdate(val, isFinished);
        }

        /// <summary>
        /// Main Bounce logic to simplify the Sample function
        /// </summary>

        float BounceLogic(float val)
        {
            if (val < 0.363636f) // 0.363636 = (1/ 2.75)
            {
                val = 7.5685f * val * val;
            }
            else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
            {
                val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
            }
            else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
            {
                val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
            }
            else
            {
                val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
            }
            return val;
        }

        /// <summary>
        /// Manually activate the tweening process, reversing it if necessary.
        /// </summary>

        public void Play(bool forward)
        {
            mAmountPerDelta = Mathf.Abs(amountPerDelta);
            if (!forward) mAmountPerDelta = -mAmountPerDelta;
        }

        /// <summary>
        /// Manually reset the tweener's state to the beginning.
        /// If the tween is playing forward, this means the tween's start.
        /// If the tween is playing in reverse, this means the tween's end.
        /// </summary>

        public void ResetToBeginning()
        {
            mFactor = (amountPerDelta < 0f) ? 1f : 0f;
            Sample(mFactor, false);
        }

        /// <summary>
        /// Manually start the tweening process, reversing its direction.
        /// </summary>

        public void Toggle()
        {
            if (mFactor > 0f)
            {
                mAmountPerDelta = -amountPerDelta;
            }
            else
            {
                mAmountPerDelta = Mathf.Abs(amountPerDelta);
            }
        }

        /// <summary>
        /// Actual tweening logic should go here.
        /// </summary>

        public System.Action<float, bool> OnUpdate;
    }
}