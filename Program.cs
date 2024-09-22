using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProcessorSimulation
{
    // Enum for instruction types
    public enum InstructionType
    {
        LOAD, STORE, ADD, SUB, JUMP, NOP, HALT
    }

    // Class to represent an Instruction
    public class Instruction
    {
        public InstructionType Type { get; set; }
        public int Operand { get; set; }

        public Instruction(InstructionType type, int operand)
        {
            Type = type;
            Operand = operand;
        }
    }

    // Class to represent the CPU
    public class CPU
    {
        private int accumulator = 0;
        private int programCounter = 0;
        private bool isHalted = false;
        private List<Instruction> instructionMemory;
        private int[] dataMemory;

        // Statistics
        private int totalCycles = 0;
        private Stopwatch executionTime;

        public CPU(List<Instruction> program, int memorySize = 16)
        {
            instructionMemory = program;
            dataMemory = new int[memorySize];
            executionTime = new Stopwatch();
        }

        // Fetch-Decode-Execute cycle
        public void Run()
        {
            executionTime.Start();
            while (!isHalted && programCounter < instructionMemory.Count)
            {
                totalCycles++;
                var instruction = Fetch();
                DecodeAndExecute(instruction);
            }
            executionTime.Stop();
            ReportResults();
        }

        // Fetch the next instruction
        private Instruction Fetch()
        {
            return instructionMemory[programCounter];
        }

        // Decode and execute the instruction
        private void DecodeAndExecute(Instruction instruction)
        {
            Console.WriteLine($"\nExecuting instruction: {instruction.Type} with operand {instruction.Operand}");

            switch (instruction.Type)
            {
                case InstructionType.LOAD:
                    accumulator = dataMemory[instruction.Operand];
                    Console.WriteLine($"Loaded {accumulator} from memory address {instruction.Operand}");
                    break;

                case InstructionType.STORE:
                    dataMemory[instruction.Operand] = accumulator;
                    Console.WriteLine($"Stored {accumulator} to memory address {instruction.Operand}");
                    break;

                case InstructionType.ADD:
                    accumulator += dataMemory[instruction.Operand];
                    Console.WriteLine($"Added {dataMemory[instruction.Operand]} to accumulator. New AC = {accumulator}");
                    break;

                case InstructionType.SUB:
                    accumulator -= dataMemory[instruction.Operand];
                    Console.WriteLine($"Subtracted {dataMemory[instruction.Operand]} from accumulator. New AC = {accumulator}");
                    break;

                case InstructionType.JUMP:
                    if (accumulator == 0)
                    {
                        programCounter = instruction.Operand - 1; // Jump to instruction (PC adjusted)
                        Console.WriteLine($"Jumping to instruction {programCounter + 1}");
                    }
                    else
                    {
                        Console.WriteLine("Jump not taken, AC not zero.");
                    }
                    break;

                case InstructionType.NOP:
                    Console.WriteLine("NOP: No operation.");
                    break;

                case InstructionType.HALT:
                    Console.WriteLine("HALT: Stopping execution.");
                    isHalted = true;
                    break;

                default:
                    throw new InvalidOperationException("Unknown instruction encountered.");
            }

            programCounter++;
        }

        // Report execution results
        private void ReportResults()
        {
            Console.WriteLine("\nExecution finished!");
            Console.WriteLine($"Total cycles: {totalCycles}");
            Console.WriteLine($"Execution time: {executionTime.ElapsedMilliseconds} ms");
            Console.WriteLine($"Final accumulator (AC) value: {accumulator}");
            Console.WriteLine($"Final program counter (PC): {programCounter}");
            Console.WriteLine("Final memory state:");
            for (int i = 0; i < dataMemory.Length; i++)
            {
                Console.WriteLine($"Memory[{i}] = {dataMemory[i]}");
            }
        }

        // Method for resetting CPU for new execution
        public void ResetCPU()
        {
            Console.WriteLine("\n--- Resetting CPU for new execution ---\n");
            accumulator = 0;
            programCounter = 0;
            isHalted = false;
            totalCycles = 0;
            executionTime.Reset();
            Array.Clear(dataMemory, 0, dataMemory.Length);
        }
    }

    // Class for Pipeline Simulation
    public class Pipeline
    {
        public void RunPipelineSimulation(List<Instruction> program)
        {
            Console.WriteLine("\n--- Starting Pipeline Simulation ---");

            // Implement basic pipeline logic similar to the Fetch-Decode-Execute cycle
            int cycles = 0;
            int pc = 0;
            int[] dataMemory = new int[16];
            int accumulator = 0;

            while (pc < program.Count)
            {
                cycles++;
                var instruction = program[pc];
                Console.WriteLine($"Cycle {cycles}: FDE - Instruction: {instruction.Type} Operand: {instruction.Operand}");

                // Simple decode and execute logic
                switch (instruction.Type)
                {
                    case InstructionType.LOAD:
                        accumulator = dataMemory[instruction.Operand];
                        break;

                    case InstructionType.STORE:
                        dataMemory[instruction.Operand] = accumulator;
                        break;

                    case InstructionType.ADD:
                        accumulator += dataMemory[instruction.Operand];
                        break;

                    case InstructionType.SUB:
                        accumulator -= dataMemory[instruction.Operand];
                        break;

                    case InstructionType.JUMP:
                        if (accumulator == 0)
                        {
                            pc = instruction.Operand - 1; // Adjusted PC for jump
                        }
                        break;

                    case InstructionType.NOP:
                        break;

                    case InstructionType.HALT:
                        Console.WriteLine("Pipeline: HALT encountered. Stopping.");
                        return;
                }

                pc++;
            }

            Console.WriteLine($"Total pipeline cycles: {cycles}");
            Console.WriteLine($"Final accumulator value: {accumulator}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Processor Simulation with Fetch-Decode-Execute Cycle!");

            // Create a sample program with a mix of instructions
            var program = new List<Instruction>
            {
                new Instruction(InstructionType.LOAD, 0),
                new Instruction(InstructionType.ADD, 1),
                new Instruction(InstructionType.STORE, 2),
                new Instruction(InstructionType.SUB, 3),
                new Instruction(InstructionType.JUMP, 5),
                new Instruction(InstructionType.NOP, 0),
                new Instruction(InstructionType.HALT, 0)
            };

            // Initialize CPU
            CPU cpu = new CPU(program);

            while (true)
            {
                // Run the CPU execution
                cpu.Run();

                // Ask user if they want to see the implemented pipeline
                Console.WriteLine("\nWould you like to see the implemented pipeline? (yes/no)");
                string pipelineResponse = Console.ReadLine().Trim().ToLower();

                if (pipelineResponse == "yes")
                {
                    Pipeline pipeline = new Pipeline();
                    pipeline.RunPipelineSimulation(program);
                }

                // Ask user if they want to reset and rerun the simulation
                Console.WriteLine("\nWould you like to reset the CPU and run the simulation again? (yes/no)");
                string response = Console.ReadLine().Trim().ToLower();

                if (response == "yes")
                {
                    cpu.ResetCPU();
                }
                else if (response == "no")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                }
            }

            Console.WriteLine("Exiting simulation.");
        }
    }
}
