using System;
using static System.Console;
using System.IO;
using System.Linq;

namespace Bme121.Pa3
{
    /// <StudentPlan>Biomedical Engineering</StudentPlan>
    /// <StudentDept>Department of Systems Design Engineering</StudentDept>
    /// <StudentInst>University of Waterloo</StudentInst>
    /// <StudentName>Tran, Megan</StudentName>
    /// <StudentUserID>mytran</StudentUserID>
    /// <StudentAcknowledgements>
    /// I declare that, except as acknowledged below, this is my original work.
    /// Acknowledged contributions of others:
    /// </StudentAcknowledgements>
    
    static partial class Program
    {
        static void Main( )
        {
            string inputFile  = @"21_training.csv";
            string outputFile = @"21_training_edges.csv";
            int height;  // image height (number of rows)
            int width;  // image width (number of columns)
            Color[ , ] inImage;
            Color[ , ] outImage;
            
            // Read the input image from its csv file.
            // Generate a file input stream to read from the csv file.
            FileStream inFile = new FileStream( inputFile, FileMode.Open, FileAccess.Read );
            StreamReader reader = new StreamReader(inFile);
            
            height = int.Parse(reader.ReadLine());
            width = int.Parse(reader.ReadLine());
            
            inImage = new Color[height, width];
            outImage = new Color[height, width];

            int a, r, g, b;

            for (int i = 0; i < height; i++)
            {
            	//For each pixel in each line, read in the 4 ARGB values at once, 
                //translate them into a Color object, and assign to the appropriate element in inImage. 

            	string[] line = reader.ReadLine().Split(',');

            	for (int j = 0; j < width; j++)
            	{
            		a = int.Parse( line[j*4] );
            		r = int.Parse( line[j*4+1] );
            		g = int.Parse( line[j*4+2] );
            		b = int.Parse( line[j*4+3] );

            		inImage[i, j] = Color.FromArgb( a, r, g, b);
            	}
            }
            
            // Generate the output image using Kirsch edge detection:
            // Get the Kirsch Edge Value for each colour from each pixel and store in the outImage array.
            for (int i = 0; i < height; i++)
            {
            	for (int j = 0; j < width; j++)
            	{
                    
                    // Pixels on the image's edges remain unchanged.
                    if (i == 0 || i == height-1 || j == 0 || j == width-1 )
                    {
                        outImage[i,j] = inImage[i,j];
                    }

                    else
                    {
                		outImage[i,j] = GetKirschEdgeValue( inImage[i-1,j-1], inImage[i-1,j], inImage[i-1,j+1],
                											inImage[i,  j-1], inImage[i  ,j], inImage[i  ,j+1],
                											inImage[i+1,j-1], inImage[i+1,j], inImage[i+1,j+1]  );
                    }
            	}
            }

            // Generate a file output stream to write the out image into the output file.
            FileStream outFile = new FileStream( outputFile, FileMode.Open, FileAccess.Write );
            StreamWriter writer = new StreamWriter(outFile);

            // Write the output image into the output file in comma-separated format.
            writer.WriteLine(height);
            writer.WriteLine(width);

            for (int i = 0; i < height; i++)
            {
            	for (int j = 0; j < width; j++)
            	{
            		writer.Write( $"{outImage[i,j].A},{outImage[i,j].R},{outImage[i,j].G},{outImage[i,j].B}," );
            	}
            	writer.WriteLine();
            }

            // Close all the streams.
            writer.Dispose();
            outFile.Dispose();
            reader.Dispose();
            inFile.Dispose();
        }
        
        // This method computes the Kirsch edge-detection value for pixel color
        // at the centre location given the centre-location pixel color and the
        // colors of its eight neighbours.  These are numbered as follows.
        // The resulting color has the same alpha as the centre pixel, 
        // and Kirsch edge-detection intensities which are computed separately
        // for each of the red, green, and blue components using its eight neighbours.
        // c1 c2 c3
        // c4    c5
        // c6 c7 c8
        static Color GetKirschEdgeValue( 
            Color c1, Color c2,     Color c3, 
            Color c4, Color centre, Color c5, 
            Color c6, Color c7,     Color c8 )
        {
            // Get the Kirsch edge-detection intensities of R,G,B.
            // Alpha value remains unchanged in the centre.
	        int red = GetKirschEdgeValue(     c1.R, c2.R, c3.R,
	            							  c4.R,       c5.R,
	            							  c6.R, c7.R, c8.R );

            int green = GetKirschEdgeValue(   c1.G, c2.G, c3.G,
	            							  c4.G, 	  c5.G,
	            							  c6.G, c7.G, c8.G );

	        int blue = GetKirschEdgeValue(	  c1.B, c2.B, c3.B,
	            							  c4.B, 	  c5.B,
	            							  c6.B, c7.B, c8.B );

            return ( Color.FromArgb( centre.A, red, green, blue ) );
        }
        
        // This method computes the Kirsch edge-detection value for pixel intensity
        // at the centre location given the pixel intensities of the eight neighbours.
        // These are numbered as follows.
        // i1 i2 i3
        // i4    i5
        // i6 i7 i8
        static int GetKirschEdgeValue( 
            int i1, int i2, int i3, 
            int i4,         int i5, 
            int i6, int i7, int i8 )
        {
            // Multiplies each permutation of the kernel to the colour value of each neighbour.
        	int kEdge1 = 	i1*5  + i2*5  + i3*5 + 
        					i4*-3 + 	    i5*-3 + 
        					i6*-3 + i7*-3 + i8*-3;

        	int kEdge2 = 	i1*-3 + i2*5  + i3*5 + 
        					i4*-3 + 		i5*5 + 
        					i6*-3 + i7*-3 + i8*-3;

        	int kEdge3 = 	i1*-3 + i2*-3 + i3*5 + 
        					i4*-3 + 		i5*5 + 
        					i6*-3 + i7*-3 + i8*5;

           	int kEdge4 = 	i1*-3 + i2*-3 + i3*-3 + 
        					i4*-3 + 	    i5*5  + 
        					i6*-3 + i7*5  + i8*5;

        	int kEdge5 = 	i1*-3 + i2*-3 + i3*-3 + 
        					i4*-3 +     	i5*-3 + 
        					i6*5 + i7*5 + i8*5;

        	int kEdge6 = 	i1*-3 + i2*-3 + i3*-3 + 
        					i4*5 + 		    i5*-3 + 
        					i6*5 +  i7*5  + i8*-3;

        	int kEdge7 = 	i1*5 + i2*-3 + i3*-3 + 
        					i4*5 + 		   i5*-3 + 
        					i6*5 + i7*-3 + i8*-3;

        	int kEdge8 = 	i1*5 +  i2*5  + i3*-3 + 
        					i4*5 + 	        i5*-3 + 
        					i6*-3 + i7*-3 + i8*-3;

            // Return the maximum value of each combination on the interval [0,255] (valid pixel values).
            int edgeValue = Math.Max( kEdge8, Math.Max (kEdge7, Math.Max(kEdge6, 
                    Math.Max(kEdge5, Math.Max(kEdge4, Math.Max(kEdge3, Math.Max(kEdge2, kEdge1) ))))));

            if (edgeValue < 0) return 0;
            if (edgeValue > 255) return 255;

            return ( edgeValue );
        }
    }
    
    // Implementation of part of System.Drawing.Color. by G. Freeman
    // This is needed because .Net Core doesn't seem to include the assembly 
    // containing System.Drawing.Color even though docs.microsoft.com claims 
    // it is part of the .Net Core API.
    struct Color
    {
        int alpha;
        int red;
        int green;
        int blue;
        
        public int A { get { return alpha; } }
        public int R { get { return red;   } }
        public int G { get { return green; } }
        public int B { get { return blue;  } }
        
        public static Color FromArgb( int alpha, int red, int green, int blue )
        {
            Color result = new Color( );
            result.alpha = alpha;
            result.red   = red;
            result.green = green;
            result.blue  = blue;
            return result;
        }
    }
}