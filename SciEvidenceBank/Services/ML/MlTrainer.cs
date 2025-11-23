//csharp SciEvidenceBank\Services\ML\MlTrainer.cs
using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace SciEvidenceBank.Services.ML
{
    // Simple example skeleton. You must create training rows from historical interactions.
    public class MlTrainer
    {
        public class TrainingRow
        {
            public string UserId { get; set; }
            public float CategoryId { get; set; }
            public float EvidenceId { get; set; }
            public float EvidencePopularity { get; set; }
            public float Label { get; set; } // 1 = positive (user interacted), 0 = negative
        }

        public void TrainAndSave(string modelPath, IEnumerable<TrainingRow> rows)
        {
            var mlContext = new MLContext(seed: 0);
            var data = mlContext.Data.LoadFromEnumerable(rows);

            // Feature pipeline: categorical encode userId, category, numerical popularity
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("UserVec", "UserId")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("CategoryVec", "CategoryId"))
                .Append(mlContext.Transforms.Concatenate("Features", "UserVec", "CategoryVec", "EvidencePopularity"))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(data);

            mlContext.Model.Save(model, data.Schema, modelPath);
        }
    }
}