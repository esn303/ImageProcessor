﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Brightness.cs" company="James South">
//   Copyright (c) James South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Encapsulates methods to change the brightness component of the image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using ImageProcessor.Common.Exceptions;

    /// <summary>
    /// Encapsulates methods to change the brightness component of the image.
    /// </summary>
    public class Brightness : IGraphicsProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Brightness"/> class.
        /// </summary>
        public Brightness()
        {
            this.Settings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets DynamicParameter.
        /// </summary>
        public dynamic DynamicParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets any additional settings required by the processor.
        /// </summary>
        public Dictionary<string, string> Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="factory">
        /// The current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class containing
        /// the image to process.
        /// </param>
        /// <returns>
        /// The processed image from the current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class.
        /// </returns>
        public Image ProcessImage(ImageFactory factory)
        {
            Bitmap newImage = null;
            Image image = factory.Image;

            try
            {
                float brightnessFactor = (float)this.DynamicParameter / 100;

                newImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppPArgb);

                ColorMatrix colorMatrix =
                    new ColorMatrix(
                        new[]
                            {
                                new float[] { 1, 0, 0, 0, 0 }, new float[] { 0, 1, 0, 0, 0 },
                                new float[] { 0, 0, 1, 0, 0 }, new float[] { 0, 0, 0, 1, 0 },
                                new[] { brightnessFactor, brightnessFactor, brightnessFactor, 0, 1 }
                            });

                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    using (ImageAttributes imageAttributes = new ImageAttributes())
                    {
                        imageAttributes.SetColorMatrix(colorMatrix);

                        graphics.DrawImage(
                            image,
                            new Rectangle(0, 0, image.Width, image.Height),
                            0,
                            0,
                            image.Width,
                            image.Height,
                            GraphicsUnit.Pixel,
                            imageAttributes);

                        image.Dispose();
                        image = newImage;
                    }
                }
            }
            catch (Exception ex)
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }

                throw new ImageProcessingException("Error processing image with " + this.GetType().Name, ex);
            }

            return image;
        }
    }
}
