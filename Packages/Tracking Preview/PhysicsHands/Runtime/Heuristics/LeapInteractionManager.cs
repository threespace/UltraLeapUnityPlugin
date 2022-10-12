using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{

    public class LeapInteractionManager : MonoBehaviour
    {
        [field: SerializeField, Tooltip("This is the original provider that your un-modified hand data should be coming from. This can be a post-process provider.")]
        public LeapProvider DataProvider { get; private set; } = null;

        [field: SerializeField, Tooltip("This should be a custom simulation provider such as a Physics Provider." +
            " Leave it as null to use the Data Provider for both sets of calculations.")]
        public SimProvider ModifiedProvider { get; private set; } = null;

        public long LastDataFrameUpdate { get; private set; } = -1;

        public long LastModifiedFrameUpdate { get; private set; } = -1;

        public FrameHeuristic DataHeuristics { get { return _dataHeuristics; } }
        [SerializeField]
        private FrameHeuristic _dataHeuristics = new FrameHeuristic();

        // Return the data heuristics if we don't have a post processor set.
        public FrameHeuristic ModifiedHeuristics { get { return ModifiedProvider == null ? _dataHeuristics : _modifiedHeuristics; } }
        [SerializeField]
        private FrameHeuristic _modifiedHeuristics = new FrameHeuristic();

        // Configurations

        [SerializeField]
        private LayerConfig _layerConfig = new LayerConfig();
        public LayerConfig LayerConfig => _layerConfig;

        public void Awake()
        { 
            InitLayers();
        }

        private void InitLayers()
        {
            _layerConfig.GenerateLayers();
            _layerConfig.SetupAutomaticCollisionLayers();
        }

        public void OnEnable()
        {
            if(DataProvider != null)
            {
                DataProvider.OnUpdateFrame += OnDataUpdate;
                DataProvider.OnFixedFrame += OnDataFixedUpdate;
            }
            if(ModifiedProvider != null)
            {
                DataProvider.OnUpdateFrame += OnModifiedUpdate;
                DataProvider.OnFixedFrame += OnModifiedFixedUpdate;
            }
        }

        public void OnDisable()
        {
            if(DataProvider != null)
            {
                DataProvider.OnUpdateFrame -= OnDataUpdate;
                DataProvider.OnFixedFrame -= OnDataFixedUpdate;
            }
            if (ModifiedProvider != null)
            {
                DataProvider.OnUpdateFrame -= OnModifiedUpdate;
                DataProvider.OnFixedFrame -= OnModifiedFixedUpdate;
            }
        }

        private void OnDataUpdate(Frame frame)
        {
            CalculateDataHeuristics(frame);
        }

        private void OnDataFixedUpdate(Frame frame)
        {
            CalculateDataHeuristics(frame);
        }

        private void CalculateDataHeuristics(Frame frame)
        {
            if (frame.Id > LastDataFrameUpdate)
            {
                LastDataFrameUpdate = (int)frame.Id;
                CalculateHeuristics(frame, ref _dataHeuristics);
            }
        }

        private void OnModifiedUpdate(Frame frame)
        {
            CalculateModifiedHeuristics(frame);
        }

        private void OnModifiedFixedUpdate(Frame frame)
        {
            CalculateModifiedHeuristics(frame);
        }

        #region Heuristics

        private void CalculateModifiedHeuristics(Frame frame)
        {
            if (frame.Id > LastModifiedFrameUpdate)
            {
                LastModifiedFrameUpdate = (int)frame.Id;
                CalculateHeuristics(frame, ref _modifiedHeuristics);
            }
        }

        private void CalculateHeuristics(Frame frame, ref FrameHeuristic heuristic)
        {
            heuristic.frameID = frame.Id;
            Hand hand = frame.GetHand(Chirality.Left);
            heuristic.leftHand.Calculate(hand);
            hand = frame.GetHand(Chirality.Right);
            heuristic.rightHand.Calculate(hand);
        }

        #endregion

        private void OnValidate()
        {
            ValidateHeuristics();
        }

        private void ValidateHeuristics()
        {
            if(DataProvider == null)
            {
                DataProvider = FindObjectOfType<LeapServiceProvider>();
            }
        }
    }

}