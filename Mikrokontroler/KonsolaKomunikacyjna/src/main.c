/**
  ******************************************************************************
  * @file    main.c
  * @author  Ac6
  * @version V1.0
  * @date    01-December-2013
  * @brief   Default main function.
  ******************************************************************************
*/

/* TODO
 *
 *
 *
 *
 *
 */

#include "stm32f10x.h"
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <stdio.h>

#define rozmiarbufora 5100
#define rozmiarbuforaprzerwanie 2000

ADC_InitTypeDef adc;


void USART2_IRQHandler();
void SysTick_Handler();

void setup();

void send_char(char);
void send_string(const char*);

void sprawdz_instrukcje(uint8_t*);
void wykonaj_instrukcje(uint8_t*);


char instrukcja[256] = "\0";
volatile static uint32_t czas = 0;
_Bool timer = false;
uint8_t kanaly = 15;

int buforczas[rozmiarbuforaprzerwanie];
uint16_t buforadc[rozmiarbufora];

unsigned int podinstrukcja = 0;
_Bool usartpodinstrukcja = false;
uint8_t sampletime = ADC_SampleTime_1Cycles5;

int main(void)
{

	setup();


	uint8_t stan = 0;

	for(;;)
	{
		sprawdz_instrukcje(&stan);
		wykonaj_instrukcje(&stan);
	}
}

void USART2_IRQHandler()
{
	static uint8_t iteracja = 0;
	if(USART_GetFlagStatus(USART2, USART_FLAG_RXNE))
	{
		usartpodinstrukcja = false;
		instrukcja[iteracja++] = USART_ReceiveData(USART2);

		if(instrukcja[iteracja-1] == '\n')
		{
			instrukcja[iteracja] = '\0';
			iteracja = 0;
			usartpodinstrukcja = true;
		}
	}
}

void SysTick_Handler()
{
	if(timer)
		czas+=2;
}

void setup()
{

	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA, ENABLE);
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO, ENABLE);
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART2, ENABLE);
	RCC_AHBPeriphClockCmd(RCC_AHBPeriph_DMA1, ENABLE);


	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB, ENABLE);
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM4, ENABLE);


	SysTick_Config((SystemCoreClock)/500000);


	RCC_ADCCLKConfig(RCC_PCLK2_Div6);
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_ADC1, ENABLE);


	GPIO_InitTypeDef gpio;
	GPIO_StructInit(&gpio);


	gpio.GPIO_Pin = GPIO_Pin_0;
	gpio.GPIO_Mode = GPIO_Mode_AIN;
	GPIO_Init(GPIOC, &gpio);

	gpio.GPIO_Pin = GPIO_Pin_1;
	gpio.GPIO_Mode = GPIO_Mode_AIN;
	GPIO_Init(GPIOC, &gpio);

	gpio.GPIO_Pin = GPIO_Pin_2;
	gpio.GPIO_Mode = GPIO_Mode_AIN;
	GPIO_Init(GPIOC, &gpio);

	gpio.GPIO_Pin = GPIO_Pin_3;
	gpio.GPIO_Mode = GPIO_Mode_AIN;
	GPIO_Init(GPIOC, &gpio);


	gpio.GPIO_Pin = GPIO_Pin_2;
	gpio.GPIO_Mode = GPIO_Mode_AF_PP;
	GPIO_Init(GPIOA, &gpio);


	gpio.GPIO_Pin = GPIO_Pin_3;
	gpio.GPIO_Mode = GPIO_Mode_IN_FLOATING;
	GPIO_Init(GPIOA, &gpio);

	//timtest
	gpio.GPIO_Pin = GPIO_Pin_6;
	gpio.GPIO_Speed = GPIO_Speed_50MHz;
	gpio.GPIO_Mode = GPIO_Mode_AF_PP;
	GPIO_Init(GPIOB, &gpio);

	TIM_TimeBaseInitTypeDef tim;
	TIM_OCInitTypeDef pwm;
	TIM_TimeBaseStructInit(&tim);
	tim.TIM_CounterMode = TIM_CounterMode_Up;
	tim.TIM_Prescaler = 640-1;
	tim.TIM_Period = 200-1;
	TIM_TimeBaseInit(TIM4, &tim);

	TIM_OCStructInit(&pwm);
	pwm.TIM_OCMode = TIM_OCMode_PWM1;
	pwm.TIM_OutputState = TIM_OutputState_Enable;
	pwm.TIM_Pulse = 100;
	TIM_OC1Init(TIM4, &pwm);
	TIM_Cmd(TIM4, ENABLE);




	USART_InitTypeDef usart;
	USART_StructInit(&usart);
	usart.USART_BaudRate = 460800;
	USART_Init(USART2, &usart);
	USART_Cmd(USART2, ENABLE);
	USART_ITConfig(USART2, USART_IT_RXNE, ENABLE);

	NVIC_InitTypeDef nvic;
	nvic.NVIC_IRQChannel = USART2_IRQn;
	nvic.NVIC_IRQChannelPreemptionPriority = 0;
	nvic.NVIC_IRQChannelSubPriority = 0;
	nvic.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&nvic);



	DMA_InitTypeDef dma;
	DMA_StructInit(&dma);
	dma.DMA_PeripheralBaseAddr = (uint32_t)&ADC1->DR;
	dma.DMA_PeripheralInc = DMA_PeripheralInc_Disable;
	dma.DMA_PeripheralDataSize = DMA_PeripheralDataSize_HalfWord;
	dma.DMA_MemoryBaseAddr = (uint32_t)&buforadc;
	dma.DMA_MemoryInc = DMA_MemoryInc_Enable;
	dma.DMA_MemoryDataSize = DMA_MemoryDataSize_HalfWord;
	dma.DMA_DIR = DMA_DIR_PeripheralSRC;
	dma.DMA_BufferSize = rozmiarbufora;
	dma.DMA_Mode = DMA_Mode_Normal;
	dma.DMA_M2M = DMA_M2M_Disable;
	dma.DMA_Priority = DMA_Priority_VeryHigh;
	DMA_Init(DMA1_Channel1, &dma);




	ADC_StructInit(&adc);
	adc.ADC_ScanConvMode = ENABLE;
	adc.ADC_ContinuousConvMode = ENABLE;
	adc.ADC_NbrOfChannel = 4;
	adc.ADC_ExternalTrigConv = ADC_ExternalTrigConv_None;
	ADC_Init(ADC1, &adc);

	ADC_InjectedSequencerLengthConfig(ADC1, 4);
	ADC_InjectedChannelConfig(ADC1, ADC_Channel_10,1 , ADC_SampleTime_1Cycles5);
	ADC_InjectedChannelConfig(ADC1, ADC_Channel_11,2 , ADC_SampleTime_1Cycles5);
	ADC_InjectedChannelConfig(ADC1, ADC_Channel_12,3 , ADC_SampleTime_1Cycles5);
	ADC_InjectedChannelConfig(ADC1, ADC_Channel_13,4 , ADC_SampleTime_1Cycles5);

	ADC_RegularChannelConfig(ADC1, ADC_Channel_10, 1, ADC_SampleTime_1Cycles5);
	ADC_RegularChannelConfig(ADC1, ADC_Channel_11, 2, ADC_SampleTime_1Cycles5);
	ADC_RegularChannelConfig(ADC1, ADC_Channel_12, 3, ADC_SampleTime_1Cycles5);
	ADC_RegularChannelConfig(ADC1, ADC_Channel_13, 4, ADC_SampleTime_1Cycles5);

	ADC_Cmd(ADC1, ENABLE);
	ADC_DMACmd(ADC1, ENABLE);
	ADC_AutoInjectedConvCmd(ADC1, ENABLE);

	ADC_ResetCalibration(ADC1);
	while(ADC_GetResetCalibrationStatus(ADC1));
	ADC_StartCalibration(ADC1);
	while(ADC_GetCalibrationStatus(ADC1));

	ADC_SoftwareStartConvCmd(ADC1, ENABLE);
	ADC_SoftwareStartInjectedConvCmd(ADC1, ENABLE);

}

void send_char(char c)
{

	while(USART_GetFlagStatus(USART2, USART_FLAG_TXE) == RESET);
	USART_SendData(USART2, c);
}

void send_string(const char* cstring)
{

	while(*cstring)
		send_char(*cstring++);
}

void sprawdz_instrukcje(uint8_t* pstan)
{
	if(strcmp(instrukcja, ":stop\r\n\0") == 0)
	{
		*pstan = 0;
		strcpy(instrukcja, "__________________________\0");
	}
	else if(strcmp(instrukcja, ":start\r\n\0") == 0)
	{
		*pstan = 1;
		strcpy(instrukcja, "__________________________\0");
	}
	else if(strcmp(instrukcja, ":info\r\n\0") == 0)
	{
		*pstan = 2;
		strcpy(instrukcja, "__________________________\0");
	}
	else if(strcmp(instrukcja, ":pstart\r\n\0") == 0)
	{
		*pstan = 3;
		strcpy(instrukcja, "__________________________\0");
	}
	else if(strcmp(instrukcja, ":dmastart\r\n\0") == 0)
	{
		*pstan = 4;
		strcpy(instrukcja, "__________________________\0");
	}
	else if(strcmp(instrukcja, ":sampletime\r\n\0") == 0)
	{
		*pstan = 5;
		strcpy(instrukcja, "__________________________\0");
	}
	else if(strcmp(instrukcja, ":channel\r\n\0") == 0)
	{
		*pstan = 6;
		strcpy(instrukcja, "__________________________\0");
	}
	else if(strcmp(instrukcja, ":nowainstrukcja\r\n\0") == 0)
	{
		*pstan = 7;
		strcpy(instrukcja, "__________________________\0");
	}
}

void wykonaj_instrukcje(uint8_t* pstan)
{
	static uint8_t zeruj = 1;
	uint16_t adc_wartosc;
	char adc_string[4];
	char adc_czas[10];
	double samplestep;

	int iter = 0;
	usartpodinstrukcja = false;

	switch(*pstan)
	{
	case 0:
	{
		timer = false;
		zeruj = 1;
		podinstrukcja = 0;
		*pstan = 8;
		break;
	}
	case 1:
	{
		ADC_AutoInjectedConvCmd(ADC1, ENABLE);

		if(zeruj)
		{
			czas = 0;
			zeruj = 0;
			timer = true;
		}

		while (!ADC_GetFlagStatus(ADC1, ADC_FLAG_EOC));
		ADC_GetConversionValue(ADC1);
		buforadc[0] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_1);
		buforadc[1] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_2);
		buforadc[2] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_3);
		buforadc[3] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_4);

		if(kanaly &0b1)
		{
			adc_wartosc = buforadc[0];

			itoa(adc_wartosc, adc_string, 10);
			itoa(czas, adc_czas, 10);
			send_string("1t");
			send_string(adc_czas);
			send_string("\r\n");
			send_string("1s");
			send_string(adc_string);
			send_string("\r\n");
		}

		if(kanaly &0b10)
		{
			adc_wartosc = buforadc[1];

			itoa(adc_wartosc, adc_string, 10);
			itoa(czas, adc_czas, 10);
			send_string("2t");
			send_string(adc_czas);
			send_string("\r\n");
			send_string("2s");
			send_string(adc_string);
			send_string("\r\n");
		}

		if(kanaly & 0b100)
		{

			adc_wartosc = buforadc[2];

			itoa(adc_wartosc, adc_string, 10);
			itoa(czas, adc_czas, 10);
			send_string("3t");
			send_string(adc_czas);
			send_string("\r\n");
			send_string("3s");
			send_string(adc_string);
			send_string("\r\n");
		}

		if(kanaly & 0b1000)
		{

			adc_wartosc = buforadc[3];

			itoa(adc_wartosc, adc_string, 10);
			itoa(czas, adc_czas, 10);
			send_string("4t");
			send_string(adc_czas);
			send_string("\r\n");
			send_string("4s");
			send_string(adc_string);
			send_string("\r\n");
		}
		break;
	}
	case 2:
	{
		send_string("Informacje o transmisji\r\n");
		send_string("USART\r\n");
		send_string("BaudRate: 460800\r\n");
		send_string("WordLength: 8b\r\n");
		send_string("StopBits: 1\r\n");
		send_string("Parity: No\r\n");
		send_string("Mode: TX, RX\r\n");
		send_string("HardwareFlowControl: None\r\n");
		*pstan = 0;
		break;
	}
	case 3:
	{
		ADC_AutoInjectedConvCmd(ADC1, ENABLE);

		while(!podinstrukcja)
			if(usartpodinstrukcja)
				podinstrukcja = strtol(instrukcja, NULL, 10);



		if(zeruj)
		{
			czas = 0;
			zeruj = 0;
			timer = true;
		}

		for(int i = 0; i < rozmiarbuforaprzerwanie*podinstrukcja/4; i++)
		{
			if(i%podinstrukcja == 0)
			{

				while (!ADC_GetFlagStatus(ADC1, ADC_FLAG_EOC));
				while (!ADC_GetFlagStatus(ADC1, ADC_FLAG_JEOC));
				ADC_GetConversionValue(ADC1);
				while (!ADC_GetFlagStatus(ADC1, ADC_FLAG_EOC));
								while (!ADC_GetFlagStatus(ADC1, ADC_FLAG_JEOC));

				buforczas[iter] = czas;
				buforadc[iter++] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_1);
				buforczas[iter] = czas;
				buforadc[iter++] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_2);
				buforczas[iter] = czas;
				buforadc[iter++] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_3);
				buforczas[iter] = czas;
				buforadc[iter++] = ADC_GetInjectedConversionValue(ADC1, ADC_InjectedChannel_4);
			}
		}

		iter = 0;

		for(int i = 0; i < rozmiarbuforaprzerwanie; i++)
		{
			itoa(buforczas[i], adc_czas, 10);
			itoa(buforadc[i], adc_string, 10);
			if(i%4 == 0)
			{
				send_string("1t");
				send_string(adc_czas);
				send_string("\r\n");
				send_string("1s");
				send_string(adc_string);
				send_string("\r\n");
			}
			else if(i%4 == 1)
			{
				send_string("2t");
				send_string(adc_czas);
				send_string("\r\n");
				send_string("2s");
				send_string(adc_string);
				send_string("\r\n");
			}
			else if(i%4 == 2)
			{
				send_string("3t");
				send_string(adc_czas);
				send_string("\r\n");
				send_string("3s");
				send_string(adc_string);
				send_string("\r\n");
			}
			else if(i%4 == 3)
			{
				send_string("4t");
				send_string(adc_czas);
				send_string("\r\n");
				send_string("4s");
				send_string(adc_string);
				send_string("\r\n");
			}
		}
		*pstan = 0;
		break;
	}
	case 4:
	{
		ADC_Cmd(ADC1, DISABLE);
		ADC_AutoInjectedConvCmd(ADC1, DISABLE);

		DMA_Cmd(DMA1_Channel1, DISABLE);
		DMA_ClearFlag(DMA_ISR_TCIF1);
		DMA_SetCurrDataCounter(DMA1_Channel1, rozmiarbufora);

		if(zeruj)
		{
			czas = 0;
			zeruj = 0;
			timer = true;
		}

		DMA_Cmd(DMA1_Channel1, ENABLE);
		ADC_Cmd(ADC1, ENABLE);
		ADC_SoftwareStartConvCmd(ADC1, ENABLE);




		while (DMA_GetFlagStatus(DMA1_FLAG_TC1) == RESET);


		timer = false;
		samplestep = czas*1.0 /rozmiarbufora;

		for(int i = 0 ; i < rozmiarbufora; i++)
		{
			itoa((int)i*samplestep, adc_czas, 10);
			itoa(buforadc[i], adc_string, 10);
			if(kanaly != 3)
			{
				if(i%4 == 0)
				{
					send_string("1t");
					send_string(adc_czas);
					send_string("\r\n");
					send_string("1s");
					send_string(adc_string);
					send_string("\r\n");
				}
				else if(i%4 == 1)
				{
					send_string("2t");
					send_string(adc_czas);
					send_string("\r\n");
					send_string("2s");
					send_string(adc_string);
					send_string("\r\n");
				}
				else if(i%4 == 2)
				{
					send_string("3t");
					send_string(adc_czas);
					send_string("\r\n");
					send_string("3s");
					send_string(adc_string);
					send_string("\r\n");
				}
				else if(i%4 == 3)
				{
					send_string("4t");
					send_string(adc_czas);
					send_string("\r\n");
					send_string("4s");
					send_string(adc_string);
					send_string("\r\n");
				}
			}
			else
			{
				if(i%2 == 0)
				{
					send_string("1t");
					send_string(adc_czas);
					send_string("\r\n");
					send_string("1s");
					send_string(adc_string);
					send_string("\r\n");
				}
				else if(i%2 == 1)
				{
					send_string("2t");
					send_string(adc_czas);
					send_string("\r\n");
					send_string("2s");
					send_string(adc_string);
					send_string("\r\n");
				}
			}
		}

		*pstan = 0;
		break;
	}
	case 5:
	{
		while(!podinstrukcja)
					if(usartpodinstrukcja)
						podinstrukcja = strtol(instrukcja, NULL, 10);
		switch(podinstrukcja)
		{
		case 1:
		{
			sampletime = ADC_SampleTime_1Cycles5;
			break;
		}
		case 2:
		{
			sampletime = ADC_SampleTime_7Cycles5;
			break;
		}
		case 3:
		{
			sampletime = ADC_SampleTime_13Cycles5;
			break;
		}
		case 4:
		{
			sampletime = ADC_SampleTime_28Cycles5;
			break;
		}
		case 5:
		{
			sampletime = ADC_SampleTime_41Cycles5;
			break;
		}
		case 6:
		{
			sampletime = ADC_SampleTime_55Cycles5;
			break;
		}
		case 7:
		{
			sampletime = ADC_SampleTime_71Cycles5;
			break;
		}
		case 8:
		{
			sampletime = ADC_SampleTime_239Cycles5;
			break;
		}
		default:
		{
			sampletime = ADC_SampleTime_1Cycles5;
			break;
		}
		}
		ADC_InjectedChannelConfig(ADC1, ADC_Channel_10,1 , sampletime);
		ADC_InjectedChannelConfig(ADC1, ADC_Channel_11,2 , sampletime);
		ADC_InjectedChannelConfig(ADC1, ADC_Channel_12,3 , sampletime);
		ADC_InjectedChannelConfig(ADC1, ADC_Channel_13,4 , sampletime);
		ADC_RegularChannelConfig(ADC1, ADC_Channel_10, 1, sampletime);
		ADC_RegularChannelConfig(ADC1, ADC_Channel_11, 2, sampletime);
		ADC_RegularChannelConfig(ADC1, ADC_Channel_12, 3, sampletime);
		ADC_RegularChannelConfig(ADC1, ADC_Channel_13, 4, sampletime);



		*pstan = 0;
		break;
	}
	case 6:
	{
		while(!podinstrukcja)
			if(usartpodinstrukcja)
				podinstrukcja = strtol(instrukcja, NULL, 10);

		if(podinstrukcja < 16)
		{
			kanaly = podinstrukcja;
			if(kanaly == 3)
			{
				adc.ADC_NbrOfChannel = 2;
				ADC_Init(ADC1, &adc);
			}
			else
			{
				adc.ADC_NbrOfChannel = 4;
				ADC_Init(ADC1, &adc);
			}
		}
		else
		{
			kanaly = 15;
			adc.ADC_NbrOfChannel = 4;
			ADC_Init(ADC1, &adc);
		}
		*pstan = 0;
		break;
	}
	case 7:
	{
		send_string("Nowa instrukcja\r\n");
		break;
	}
	default:
	{
		break;
	}

	}
}
