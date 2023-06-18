using System.Collections.Generic;
using UnityEngine;


public class WorldSpawner : MonoBehaviour
{
    [SerializeField] private Transform _startSpawnPosition;   

    [Header("RoadMarks")]
    [SerializeField] private RoadSign _roadSignPrefab;

    [Header("GroundSegment")]
    [SerializeField] private GroundSegment _groundSegmentPrefab;
    [SerializeField] private int _countOfLinesOnSegment;
    [SerializeField] private int _countOfSegments;
    [SerializeField] private float _distanceBetweenLines;


    [Header("Line")]
    [SerializeField] RoadMark _linePrefab;
    [SerializeField] private float _lineWidth;

    [Header("Block")]
    [SerializeField] Block _blockPrefab;

    [Range(0, 100)]
    [SerializeField] int _blockSpawnChance;
    [SerializeField] private float _xDistanceBetweenBlocks;

    [Header("Bonus")]
    [SerializeField] Bonus _bonusPrefab;
    [Range(0, 100)]
    [SerializeField] int _bonusSpawnChance;

    [Header("Ground collider")]
    [SerializeField] private GroundSegment _colliderGround;

    [Header("SpawnPoints")]
    [SerializeField] private int _spawnPointsCount;

    [Header("SpringBoard")]
    [SerializeField] private SpringBoard _springBoardPrefab;
    [SerializeField] private float _springBoardLength;
    [Range(0, 100)]
    [SerializeField] private int _springBoardSpawnChance;

    private float _cameraHeight;

    private float _screenWidth;

    private float _distanceBetweenPoints;

    private GameObject _parent;

    private Vector3[] _spawnPointsSample;

    private List<GroundSegment> _spawnedGroundSegments = new List<GroundSegment>();
    private Dictionary<GroundSegment, Block[,]> _segmentsWithBlocks = new Dictionary<GroundSegment, Block[,]>();
    private Dictionary<GroundSegment, Bonus[,]> _segmentsWithBonuses = new Dictionary<GroundSegment, Bonus[,]>();
    private Dictionary<GroundSegment, SpringBoard[,]> _segmentWithTramplins = new Dictionary<GroundSegment, SpringBoard[,]>();
    private Dictionary<GroundSegment, SpawnPoint[,]> _segmentsWithPoints = new Dictionary<GroundSegment, SpawnPoint[,]>();



    private void Awake()
    {

        _parent = new GameObject("Enviroment");

        _cameraHeight = Camera.main.GetComponent<TopDownCamera>().CameraHeight;

        _spawnPointsSample = CreateSpawnPointsSample(_spawnPointsCount, _startSpawnPosition.position, _cameraHeight, 1f);



        for (int i = 0; i < _countOfSegments; i++)
        {
            var segment = SpawnGroundSegment(_countOfLinesOnSegment, _startSpawnPosition.position, _distanceBetweenLines, _groundSegmentPrefab, i);
            _spawnedGroundSegments.Add(segment);


            Block[,] blocks = SpawnObstacles<Block>(_blockPrefab, _countOfLinesOnSegment, segment, _spawnPointsSample, _distanceBetweenLines);
            Bonus[,] bonuses = SpawnObstacles<Bonus>(_bonusPrefab, _countOfLinesOnSegment, segment, _spawnPointsSample, _distanceBetweenLines);
            SpringBoard[,] springBoards = SpawnObstacles<SpringBoard>(_springBoardPrefab, _countOfLinesOnSegment, segment, _spawnPointsSample, _distanceBetweenLines, _distanceBetweenLines / 2f);

            SetObstaclesScale(new Vector3(_distanceBetweenPoints - _xDistanceBetweenBlocks, _blockPrefab.transform.localScale.y, _blockPrefab.transform.localScale.z),blocks);
            SetObstaclesScale(new Vector3(_distanceBetweenPoints - _xDistanceBetweenBlocks, _distanceBetweenPoints - _xDistanceBetweenBlocks, _distanceBetweenPoints - _xDistanceBetweenBlocks),bonuses);
            SetObstaclesScale(new Vector3(_distanceBetweenPoints, _springBoardPrefab.transform.localScale.y, _springBoardLength), springBoards);

            _segmentsWithBlocks.Add(segment, blocks);
            _segmentsWithBonuses.Add(segment, bonuses);
            _segmentWithTramplins.Add(segment, springBoards);


            foreach (var block in _segmentsWithBlocks[segment])
            {
                DrawRoadSigns(segment, block, _roadSignPrefab, _distanceBetweenPoints * 0.6f, SignType.Stop, -(_distanceBetweenLines - _distanceBetweenPoints));
            }

            foreach (var bonus in _segmentsWithBonuses[segment])
            {
                DrawRoadSigns(segment, bonus, _roadSignPrefab, _distanceBetweenPoints*0.6f, SignType.Bonus, -(_distanceBetweenLines - _distanceBetweenPoints));

            }

            DrawRoadMarkings(segment, _spawnPointsSample, _linePrefab, _distanceBetweenPoints, 0.3f);

            _segmentsWithPoints.Add(segment, CreatAllSpawnPointsOnSegment(_countOfLinesOnSegment, _spawnPointsSample, segment, _distanceBetweenLines));
            SpawnPointsMakeEmpty(_segmentsWithPoints[segment]);

            ShowRandomObstacles(_blockSpawnChance, _segmentsWithBlocks[segment], _segmentsWithPoints[segment]);
            ShowRandomObstacles(_bonusSpawnChance, _segmentsWithBonuses[segment], _segmentsWithPoints[segment]);
            ShowRandomObstacles(_springBoardSpawnChance, _segmentWithTramplins[segment], _segmentsWithPoints[segment], true);

        }       

         _colliderGround = SpawnGroundCollider(_groundSegmentPrefab, _startSpawnPosition.position, _spawnedGroundSegments);
        TransformGroundCollider();

    }

    private void OnDisable()
    {
        foreach(var groundSegment in _spawnedGroundSegments)
        {
            groundSegment.SegmentBecameInvisibleEvent -= OnSegmentBecameInvisible;
        }
    }

    #region "GroundSegment"

    private GroundSegment SpawnGroundSegment(int countOflinesOnSegment, Vector3 spawnPosition, float distanceBetweenLines, GroundSegment segmentPrefab, int count)
    {

        var position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z + count * distanceBetweenLines * countOflinesOnSegment);

        GroundSegment groundSegment = Instantiate(segmentPrefab, position, Quaternion.identity, _parent.transform);
       
        groundSegment.View.transform.localScale = new Vector3(_screenWidth / 10f, 1f, distanceBetweenLines * countOflinesOnSegment / 10f);

        groundSegment.name = count.ToString();
        groundSegment.SegmentBecameInvisibleEvent += OnSegmentBecameInvisible;


        return groundSegment;

    }

    private GroundSegment SpawnGroundCollider(GroundSegment groundSegmentPrefab, Vector3 spawnPosition, List<GroundSegment> allGroundSegments)
    {
        var colliderGround = Instantiate(groundSegmentPrefab, spawnPosition, Quaternion.identity);
        colliderGround.View.SetVisible(false);
        colliderGround.View.AddCollider();
        colliderGround.transform.parent = _parent.transform;

        Vector3 scale = new Vector3(allGroundSegments[0].View.transform.localScale.x, allGroundSegments[0].View.transform.localScale.y, allGroundSegments[0].View.transform.localScale.z * allGroundSegments.Count);
        colliderGround.View.transform.localScale = scale;

        return colliderGround;
    }

    private void TransformGroundCollider()
    {

        Vector3 firstSegmentLeftside = new Vector3(_spawnedGroundSegments[0].transform.position.x, _spawnedGroundSegments[0].transform.position.y, _spawnedGroundSegments[0].transform.position.z - _spawnedGroundSegments[0].View.transform.localScale.z * 10f / 2f);
        Vector3 position = new Vector3(firstSegmentLeftside.x, firstSegmentLeftside.y, firstSegmentLeftside.z + (_spawnedGroundSegments[0].View.transform.localScale.z * 10f * _spawnedGroundSegments.Count) / 2f);
        _colliderGround.transform.position = position;

    }

    private void OnSegmentBecameInvisible(GroundSegment groundSegment)
    {

        var newPosition = _spawnedGroundSegments[_spawnedGroundSegments.Count - 1].transform.position;
        var currentSegment = _spawnedGroundSegments[_spawnedGroundSegments.Count - 1];
        groundSegment.transform.position = new Vector3(newPosition.x, newPosition.y, newPosition.z + currentSegment.View.transform.localScale.z * 10f);
        _spawnedGroundSegments.Remove(groundSegment);
        _spawnedGroundSegments.Add(groundSegment);


        SpawnPointsMakeEmpty(_segmentsWithPoints[groundSegment]);
        ShowRandomObstacles(_blockSpawnChance, _segmentsWithBlocks[groundSegment], _segmentsWithPoints[groundSegment]);
        ShowRandomObstacles(_bonusSpawnChance, _segmentsWithBonuses[groundSegment], _segmentsWithPoints[groundSegment]);
        ShowRandomObstacles(_springBoardSpawnChance, _segmentWithTramplins[groundSegment], _segmentsWithPoints[groundSegment], true);

        TransformGroundCollider();
    }

    #endregion

    #region "Obstacles"

    private T[,] SpawnObstacles<T>(PoolObj objPrefab, int countOflinesOnSegment, GroundSegment segment, Vector3[] spawnPointsSample, float distanceBetweenLines, float zOffSet = 0f)
    {

        if (countOflinesOnSegment == 0)
        {
            return null;
        }

        if (objPrefab.GetComponent<T>() == null)
        {

            return null;
        }

        Vector3 segmentLeftSide = new Vector3(segment.transform.position.x, segment.transform.position.y, segment.transform.position.z - segment.View.transform.localScale.z * 10f / 2f);


        T[,] collidableObjsOnSegment = new T[countOflinesOnSegment, spawnPointsSample.Length];
        for (int i = 0; i < countOflinesOnSegment; i++)
        {

            for (int j = 0; j < spawnPointsSample.Length; j++)
            {
                Vector3 spawnpoitPosition = spawnPointsSample[j];
                spawnpoitPosition = new Vector3(spawnpoitPosition.x, spawnpoitPosition.y, segmentLeftSide.z + distanceBetweenLines * i + zOffSet);

                var obj = Instantiate(objPrefab, spawnpoitPosition, objPrefab.transform.rotation);              

                obj.transform.parent = segment.transform;
                obj.SetActive(false);

                var obj2 = obj.GetComponent<T>();
                collidableObjsOnSegment[i, j] = obj2;

            }
        }
        return collidableObjsOnSegment;
    }

    private void SetObstaclesScale(Vector3 scale, PoolObj[,] objs)
    {

        foreach (var obj in objs)
        {
            var parent = obj.transform.parent;
            obj.transform.parent = null;

            obj.transform.localScale = scale;

            obj.transform.parent = parent;

        }

    }

    private void ShowRandomObstacles(int spawnChance, PoolObj[,] objects, SpawnPoint[,] points, bool isPlacebleOnFilledPoint = false)
    {
        if (objects.GetLength(0) != points.GetLength(0) || objects.GetLength(1) != points.GetLength(1))
        {
            Debug.LogError("points[,] dictionary equal Collidableobj[,] dictionary");
        }

        foreach (var obj in objects)
        {
            obj.SetActive(false);
        }


        for (int i = 0; i < points.GetLength(0); i++)
        {
            for (int j = 0; j < points.GetLength(1); j++)
            {
                if (Random.Range(0, 100) < spawnChance && (points[i, j].IsEmpty == true || isPlacebleOnFilledPoint))
                {
                    objects[i, j].SetActive(true);

                    points[i, j].SetEmptiness(false);

                }
            }
        }
    }

    #endregion

    #region "SpawnPoints"

    private Vector3[] CreateSpawnPointsSample(int count, Vector3 carStartPosition, float cameraHeight, float yOffset = 0f)
    {
        Vector3[] spawnpoints = new Vector3[count];

        var cam = Camera.main;

        Vector3 bottom = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cameraHeight - yOffset));
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0f, 1f, cameraHeight - yOffset));

        float width = (top - bottom).magnitude;

        _screenWidth = width;

        float distanceBetweenpoints = width / (count + 1f);

        float xEdgeOffset = distanceBetweenpoints / 2f + distanceBetweenpoints / (2f * (count));

        float addValue = 2f * (distanceBetweenpoints - xEdgeOffset) / (count - 1);
        distanceBetweenpoints += addValue;

        _distanceBetweenPoints = distanceBetweenpoints;

        var center = (bottom + top) / 2f;

        _startSpawnPosition.position = center;      

        for (int i = 0; i < count; i++)
        {

            spawnpoints[i].y = center.y;
            if (i == 0)
            {
                spawnpoints[i].x = bottom.x - xEdgeOffset;
            }
            else
            {
                spawnpoints[i].x = spawnpoints[i - 1].x - distanceBetweenpoints;
            }
        }

        return spawnpoints;
    }


    private SpawnPoint[,] CreatAllSpawnPointsOnSegment(int countOflinesOnSegment, Vector3[] spawnPointsSample, GroundSegment segment, float distanceBetweenLines)
    {
        if (countOflinesOnSegment == 0)
        {
            return null;
        }
        Vector3 segmentLeftSide = new Vector3(segment.transform.position.x, segment.transform.position.y, segment.transform.position.z - segment.View.transform.localScale.z * 10f / 2f);

        SpawnPoint[,] pointsOnSegment = new SpawnPoint[countOflinesOnSegment, spawnPointsSample.Length];

        for (int i = 0; i < countOflinesOnSegment; i++)
        {

            for (int j = 0; j < spawnPointsSample.Length; j++)
            {
                Vector3 spawnpoitPosition = spawnPointsSample[j];
                spawnpoitPosition = new Vector3(spawnpoitPosition.x, spawnpoitPosition.y, segmentLeftSide.z + distanceBetweenLines * i);
                spawnpoitPosition = segment.transform.InverseTransformPoint(spawnpoitPosition);

                pointsOnSegment[i, j] = new SpawnPoint(spawnpoitPosition);
            }
        }

        return pointsOnSegment;
    }

    private void SpawnPointsMakeEmpty(SpawnPoint[,] points)
    {
        foreach (var point in points)
        {
            point.SetEmptiness( true);
        }
    }


    #endregion

    #region "RoadMarks"
    private RoadSign DrawRoadSigns(GroundSegment segment, PoolObj obj, RoadSign roadSignPrefab, float scale, SignType signType, float zOffset)
    {
        Vector3 position = obj.transform.position;
        position = new Vector3(position.x, position.y, position.z + zOffset);
        var sign = Instantiate(roadSignPrefab, position, roadSignPrefab.transform.rotation);

        sign.SetMarkType(signType);
        sign.transform.localScale = new Vector3(scale, scale, 1);

        sign.transform.parent = segment.transform;

        sign.SubscribeToOwner(obj);

        return sign;
    }


    private void DrawRoadMarkings(GroundSegment segment, Vector3[] spawnPointsSample, RoadMark linePrefab, float distanceBetweenPoints, float lineWidth)
    {
        Vector3 segmentLeftSide = new Vector3(segment.transform.position.x, segment.transform.position.y, segment.transform.position.z - segment.View.transform.localScale.z * 10f / 2f);


        for (int i = 0; i <= spawnPointsSample.Length; i++)
        {
            var position = new Vector3(spawnPointsSample[0].x +
                distanceBetweenPoints / 2f - distanceBetweenPoints * i, segmentLeftSide.y + 0.01f, segmentLeftSide.z);

            var line = Instantiate(linePrefab, position, linePrefab.transform.rotation);

            line.Setscale(lineWidth, segment.View.transform.localScale.z * 10f);
            line.transform.parent = segment.transform;

        }
    }
    #endregion


}
