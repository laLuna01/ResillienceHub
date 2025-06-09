using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;

namespace ResilienceHub.Infrastructure.Services;
public class DisasterData
{
    [LoadColumn(0)]
    public float HistoricoDesastres { get; set; }

    [LoadColumn(1)]
    public float PopulacaoAfetada { get; set; }

    [LoadColumn(2)]
    public bool InfraestruturaCritica { get; set; }

    [LoadColumn(3)]
    public string TipoDesastre { get; set; }

    [LoadColumn(4)]
    [ColumnName("Label")]
    public string NivelRisco { get; set; }
}

public class DisasterPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedNivelRisco { get; set; }
}

public class MLService
{
    private readonly MLContext _mlContext;
    private ITransformer _trainedModel;

    public MLService(string dataPath)
    {
        _mlContext = new MLContext(seed: 0);
        TrainModel(dataPath);
    }

    private void TrainModel(string dataPath)
    {
        IDataView dataView = _mlContext.Data.LoadFromTextFile<DisasterData>(dataPath, separatorChar: ',', hasHeader: true);

        var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("TipoDesastre")
            .Append(_mlContext.Transforms.Concatenate("Features", "HistoricoDesastres", "PopulacaoAfetada", "InfraestruturaCritica", "TipoDesastre"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("Label"))
            .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        _trainedModel = pipeline.Fit(dataView);
    }

    public DisasterPrediction Predict(DisasterData input)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<DisasterData, DisasterPrediction>(_trainedModel);
        return predictionEngine.Predict(input);
    }
}